using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PageBuilderModel : PageModelBase
    {
        #region Methods
        public ViewModel GetViewModel(int? pageID, string languageCultureCode)
        {
            if (string.IsNullOrWhiteSpace(languageCultureCode))
            {
                languageCultureCode = Enums.Languages.GEORGIAN;
            }

            var viewModel = new ViewModel();
            viewModel.PageTitle = Utilities.GetValuesByLanguage(languageCultureCode, Page.PageTitle, Page.PageTitleEng);
            viewModel.PageText = Utilities.GetValuesByLanguage(languageCultureCode, Page.PageText, Page.PageTextEng);
            viewModel.PageData = Utilities.GetValuesByLanguage(languageCultureCode, Page.PageData, Page.PageDataEng) ?? "[]";
            viewModel.IsPublished = Page.PageIsPublished;
            viewModel.Language = languageCultureCode;
            viewModel.UrlBack = Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.Properties, new { pageID = Page.PageID });
            viewModel.UrlPreview = GetRouteByName(ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = Page.PageSlug });
            viewModel.UrlSave = UrlCurrentPageWithDomain;
            viewModel.UrlFileManager = UrlFactory.CreateFileManagerAdminUrl(fileManagerModule: FileManagerModules.Pages);

            viewModel.SelectedLanguage = Utilities.GetValuesByLanguage(languageCultureCode, Enums.Languages.GEORGIAN, Enums.Languages.ENGLISH);
            viewModel.LanguageOptions =
            [
                new() { Key = nameof(Enums.Languages.GEORGIAN), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.PageBuilderController.BuilderLanguage, new { pageID, Language = Enums.Languages.GEORGIAN }), IsSelected = languageCultureCode == Enums.Languages.GEORGIAN },
                new() { Key = nameof(Enums.Languages.ENGLISH), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.PageBuilderController.BuilderLanguage, new { pageID, Language = Enums.Languages.ENGLISH }), IsSelected = languageCultureCode == Enums.Languages.ENGLISH },                
            ];

            viewModel.PluginsClient = new PluginsClientViewModel();
            return viewModel;
        }

        public ValidationResult63 Validate(SubmitModel submitModel)
        {
            var validationResult = new ValidationResult63();
            validationResult.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.PageTitle)), valueToValidate: submitModel.PageTitle));
            return validationResult;
        }

        public async Task<AjaxResponse> Save(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreatePagesRepository();

            switch (submitModel.Language)
            {
                case Enums.Languages.GEORGIAN:
                    {
                        await repository.PagesIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            pageID: Page.PageID,
                            page: new PageIudDTO
                            {
                                PageText = submitModel.PageText ?? Constants.NullValueFor.String,
                                PageTextHeaderHtml = submitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                                PageTextFooterHtml = submitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                                PageData = submitModel.PageData ?? Constants.NullValueFor.String
                            }
                        );
                        break;
                    }
                case Enums.Languages.ENGLISH:
                    {
                        await repository.PagesIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            pageID: Page.PageID,
                            page: new PageIudDTO
                            {
                                PageTextEng = submitModel.PageText ?? Constants.NullValueFor.String,
                                PageTextHeaderHtmlEng = submitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                                PageTextFooterHtmlEng = submitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                                PageDataEng = submitModel.PageData ?? Constants.NullValueFor.String
                            }
                            
                        );
                        break;
                    }                
            }
            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public PluginsClientViewModel PluginsClient { get; set; }
            public string PageTitle { get; set; }
            public string PageText { get; set; }
            public bool IsPublished { get; set; }
            public string Language { get; set; }
            public string PageData { get; set; }
            public string UrlBack { get; set; }
            public string UrlPreview { get; set; }
            public string UrlSave { get; set; }
            public string UrlFileManager { get; set; }            
            public string SelectedLanguage { get; set; }
            public List<KeyValueSelectedTuple<string, string>> LanguageOptions { get; set; }
            public bool HasLanguageOptions => LanguageOptions?.Count > 0;
            
            public readonly string TextError = Resources.TextError;
            #endregion
        }

        public class SubmitModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string PageSlug { get; set; }
            public string PageText { get; set; }
            public string Language { get; set; }
            public string PageData { get; set; }
            public string HeaderSectionHtml { get; set; }
            public string FooterSectionHtml { get; set; }
            public bool IsPublished { get; set; }
            #endregion
        }
        #endregion
    }
}