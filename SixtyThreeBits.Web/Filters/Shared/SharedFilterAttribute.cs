using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Shared
{
    public class SharedFilterAttribute : IAsyncActionFilter
    {
        AppSettingsCollection _appSettings;
        UtilityCollection _utilities;
        RepositoryFactory _dataAccessFactory;
        ModelBase _model;

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
                _model.AppSettings = _appSettings;
                _model.Utilities = _utilities;
                _model.RepositoriesFactory = _dataAccessFactory;

                var ActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;

                _model.ActionName = ActionDescriptor.ActionName;
                _model.ControllerName = ActionDescriptor.ControllerTypeInfo.Name;

                _model.WebsiteDomain = WebUtilities.GetWebsiteDomain(c.Request);
                _model.UrlCurrentPageWithoutDomain = c.Request.Path;
                _model.UrlCurrentPageWithDomain = $"{_model.WebsiteDomain}{c.Request.Path}";

                _model.IsHttps = c.Request.IsHttps;
                _model.IP = WebUtilities.GetClientIP(c.Request);

                _model.SessionAssistance = new SessionAssistance(c.HttpContext.Session);
                _model.CookieAssistance = new CookieAssistance(c.Request, c.Response);
                _model.Url = c.Url;
                _model.Request = c.Request;
                _model.Response = c.Response;
                _model.UrlPreviousPage = _model.Request.Headers["Referer"].ToString();
                _model.Form = new FormViewModelBase();



                _model.LanguageCultureCode = _model.Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

                _model.PluginsClient = new PluginsClient(_model.LanguageCultureCode);

                await InitSystemProperties();
                InitFileStorage();
                await InitUser();
                await next();
            }
        }

        async Task InitUser()
        {
            _model.User = _model.SessionAssistance.Get<UserDTO>(WebConstants.Session.User);

            if (_model.User == null)
            {
                var userID = _model.CookieAssistance.Get<int?>(WebConstants.Cookies.User);
                if (userID != null)
                {
                    var repository = _dataAccessFactory.GetUsersRepository();
                    _model.User = await repository.UsersGetSingleUserByUserID(userID);
                    _model.SessionAssistance.Set(WebConstants.Session.User, _model.User);
                }
            }
        }

        async Task InitSystemProperties()
        {
            var repository = _dataAccessFactory.GetSystemPropertiesRepository();
            _model.SystemProperties = await repository.SystemPropertiesGet();
        }

        void InitFileStorage()
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
    }
}

