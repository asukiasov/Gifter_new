using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class TeamMemberPropertiesModel : TeamMembersModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.TeamMemberFirstname = DBItem.TeamMemberFirstname;
                viewModel.TeamMemberLastname = DBItem.TeamMemberLastname;
                viewModel.TeamMemberPosition = DBItem.TeamMemberPosition;
                viewModel.TeamMemberShortDescription = DBItem.TeamMemberShortDescription;
                viewModel.TeamMemberLongDescription = DBItem.TeamMemberLongDescription;
                viewModel.TeamMemberIsPublished = DBItem.TeamMemberIsPublished;
                viewModel.TeamMemberCategoryID = DBItem.TeamMemberCategoryID;
            }

            var repository = RepositoriesFactory.CreateDictionariesRepository();
            viewModel.TeamMemberCategories = await repository.DictionariesListAsKeyValueSelectedTuple(dictionaryCode: Enums.DictionaryCodes.TeamMemberCategories, selectedValue: viewModel.TeamMemberCategoryID);
            viewModel.TeamMemberImageFilename = DBItem.TeamMemberImageFilename;
            viewModel.TeamMemberImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.TeamMemberImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMemberPropertiesController.DeleteImage, new { teamMemberID = DBItem.TeamMemberID });
            return viewModel;
        }
        
        public async Task<ViewModel> Save(ViewModel submitModel)
        {
            var viewModel = await GetViewModel(submitModel);

            var validationResult = validateSubmitModel(submitModel);

            if (validationResult.HasErrors)
            {
                viewModel.AddFormErrors(validationResult.Errors);
            }
            else
            {
                var hasTeamMemberImage = submitModel.TeamMemberImageFile?.Length > 0;
                var teamMemberImageFilename = hasTeamMemberImage ? GetFilenameFromUploadedFile(submitModel.TeamMemberImageFile) : null;
                if (hasTeamMemberImage)
                {
                    await FileStorage.DeleteFile(DBItem.TeamMemberImageFilename);
                }

                var repository = RepositoriesFactory.CreateTeamMembersRepository();
                await repository.TeamMembersIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    teamMemberID: DBItem.TeamMemberID,
                    teamMember: new TeamMemberIudDTO
                    {
                        TeamMemberFirstname = submitModel.TeamMemberFirstname,
                        TeamMemberLastname = submitModel.TeamMemberLastname,
                        TeamMemberPosition = submitModel.TeamMemberPosition ?? Constants.NullValueFor.String,
                        TeamMemberShortDescription = submitModel.TeamMemberShortDescription ?? Constants.NullValueFor.String,
                        TeamMemberLongDescription = submitModel.TeamMemberLongDescription ?? Constants.NullValueFor.String,
                        TeamMemberImageFilename = teamMemberImageFilename,
                        TeamMemberIsPublished = submitModel.TeamMemberIsPublished,
                        TeamMemberCategoryID = submitModel.TeamMemberCategoryID
                    }
                );

                if (repository.IsError)
                {
                    submitModel.AddToastError(repository.ErrorMessage);
                }
                else
                {
                    if (hasTeamMemberImage)
                    {
                        await FileStorage.SaveUploadedFile(
                            sourceFileStream: submitModel.TeamMemberImageFile.OpenReadStream(),
                            filename: teamMemberImageFilename
                        );
                    }
                }
            }

            return viewModel;
        }

        ValidationResult63 validateSubmitModel(ViewModel submitModel)
        {
            var validationResult = new ValidationResult63();
            var error = default(Error63);

            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.TeamMemberFirstname)), valueToValidate: submitModel.TeamMemberFirstname);
            validationResult.AddError(error);

            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.TeamMemberLastname)), valueToValidate: submitModel.TeamMemberLastname);
            validationResult.AddError(error);

            return validationResult;
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            await FileStorage.DeleteFile(DBItem.TeamMemberImageFilename);
            var repository = RepositoriesFactory.CreateTeamMembersRepository();
            await repository.TeamMembersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                teamMemberID: DBItem.TeamMemberID,
                teamMember: new TeamMemberIudDTO
                {
                    TeamMemberImageFilename = Constants.NullValueFor.String
                }
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
        {
            #region Properties             
            public string TeamMemberFirstname { get; set; }
            public string TeamMemberLastname { get; set; }
            public string TeamMemberPosition { get; set; }
            public string TeamMemberShortDescription { get; set; }
            public string TeamMemberLongDescription { get; set; }
            public string TeamMemberImageFilename { get; set; }
            public bool TeamMemberIsPublished { get; set; }
            public int? TeamMemberCategoryID { get; set; }
            public bool HasTeamMemberCategories => TeamMemberCategories?.Count > 0;
            public List<KeyValueSelectedTuple<int?, string>> TeamMemberCategories { get; set; }
            public string TeamMemberImageHttpPath { get; set; }
            public bool HasTeamMemberImage => !string.IsNullOrWhiteSpace(TeamMemberImageFilename);
            public string UrlDeleteImage { get; set; }
            public IFormFile TeamMemberImageFile { get; set; }

            public readonly int NullValueForInt = Constants.NullValueFor.Numeric;

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextFirstname = Resources.TextFirstname;
            public readonly string TextLastname = Resources.TextLastname;
            public readonly string TextPosition = Resources.TextPosition;
            public readonly string TextCategory = Resources.TextCategory;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescription = Resources.TextDescription;
            #endregion
        }
        #endregion
    }
}
