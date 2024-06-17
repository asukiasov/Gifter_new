using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Shared
{
    public class SharedFilterAttribute : IAsyncActionFilter
    {
        #region Properties
        AppSettingsCollection _appSettings;
        UtilityCollection _utilities;
        RepositoryFactory _dataAccessFactory;
        ModelBase _model;
        #endregion

        #region Methods
        public SharedFilterAttribute(AppSettingsCollection appSettings, UtilityCollection utilities, RepositoryFactory dataAccessFactory)
        {
            _appSettings = appSettings;
            _utilities = utilities;
            _dataAccessFactory = dataAccessFactory;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            var c = filterContext.Controller as Controller;
            _model = WebUtilities.GetModelFromController<ModelBase>(c);
            if (_model != null)
            {
                initCulture(c);
                initModelBaseProperties(filterContext, c);
                await initSystemProperties();
                initFileStorage();
                await initUser();
                await next();
            }
        }

        void initCulture(Controller c)
        {
            _model.LanguageCultureCode = c.Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
        }

        void initModelBaseProperties(ActionExecutingContext filterContext, Controller c)
        {
            var actionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;

            _model.AppSettings = _appSettings;
            _model.Utilities = _utilities;
            _model.RepositoriesFactory = _dataAccessFactory;
            
            _model.Controller = c;
            _model.ActionName = actionDescriptor.ActionName;
            _model.ControllerName = actionDescriptor.ControllerTypeInfo.Name;

            _model.WebsiteDomain = WebUtilities.GetWebsiteDomain(c.Request);
            _model.UrlCurrentPageWithoutDomain = c.Request.Path;
            _model.UrlCurrentPageWithDomain = $"{_model.WebsiteDomain}{c.Request.Path}";

            _model.IsHttps = c.Request.IsHttps;
            _model.IsAjaxRequest = c.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            _model.IP = WebUtilities.GetClientIP(c.Request);

            _model.SessionAssistance = new SessionAssistance(c.HttpContext.Session);
            _model.CookieAssistance = new CookieAssistance(c.Request, c.Response);
            _model.Url = c.Url;
            _model.Request = c.Request;
            _model.Response = c.Response;
            _model.UrlPreviousPage = _model.Request.Headers["Referer"].ToString();
            _model.Form = new FormViewModelBase();

            _model.PluginsClient = new PluginsClientViewModel(_model.LanguageCultureCode);
        }

        async Task initUser()
        {
            _model.User = _model.SessionAssistance.Get<UserDTO>(WebConstants.Session.User);

            if (_model.User == null)
            {
                var userIDEncrypted = _model.CookieAssistance.Get(WebConstants.Cookies.User);
                var userID = userIDEncrypted.AesDecryptString().ToInt();
                if (userID != null)
                {
                    var repository = _dataAccessFactory.CreateUsersRepository();
                    _model.User = await repository.UsersGetSingleByID(userID);
                    _model.SessionAssistance.Set(WebConstants.Session.User, _model.User);
                }
            }
        }

        async Task initSystemProperties()
        {
            var repository = _dataAccessFactory.CreateSystemPropertiesRepository();
            _model.SystemProperties = await repository.SystemPropertiesGet();
        }

        void initFileStorage()
        {
            _model.FileStorage = new LocalFileStorage(
                uploadFolderPhysicalPath: _appSettings.UploadFolderPhysicalPath,
                uploadFolderHttpPath: _appSettings.UploadFolderHttpPath,
                noImageHttpPath: "/images/no-image.jpg",
                websiteDomain: _model.WebsiteDomain
           );

            //_model.FileStorage = new AwsFileStorage(
            //    awsAccessKeyID: _model.SystemProperties.AwsAccessKeyID,
            //    awsSecretAccessKey: _model.SystemProperties.AwsSecretAccessKey,
            //    awsS3RegionSystemName: _model.SystemProperties.AwsS3RegionSystemName,
            //    awsS3BucketNamePublic: _model.SystemProperties.AwsS3BucketNamePublic,
            //    noImageHttpPath: "/images/no-image.jpg"
            //);

            //_model.FileStorage = new AzureFileStorage(
            //    azureBlobStorageConnectionString: _model.SystemProperties.AzureConnectionString,
            //    azureBlobStorageContainerName: _model.SystemProperties.AzureBlobStorageContainerName,
            //    noImageHttpPath: "/images/no-image.jpg"
            //);
        } 
        #endregion
    }
}