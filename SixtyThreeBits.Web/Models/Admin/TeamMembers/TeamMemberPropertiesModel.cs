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

        public void Validate(ViewModel viewModel)
        {
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.TeamMemberFirstname)), valueToValidate: viewModel.TeamMemberFirstname));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.TeamMemberLastname)), valueToValidate: viewModel.TeamMemberLastname));
        }

        public async Task Save(ViewModel viewModel)
        {
            var hasTeamMemberImage = viewModel.TeamMemberImageFile?.Length > 0;
            var teamMemberImageFilename = hasTeamMemberImage ? GetFilenameFromUploadedFile(viewModel.TeamMemberImageFile) : null;
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
                    TeamMemberFirstname = viewModel.TeamMemberFirstname,
                    TeamMemberLastname = viewModel.TeamMemberLastname,
                    TeamMemberPosition = viewModel.TeamMemberPosition ?? Constants.NullValueFor.String,
                    TeamMemberShortDescription = viewModel.TeamMemberShortDescription ?? Constants.NullValueFor.String,
                    TeamMemberLongDescription = viewModel.TeamMemberLongDescription ?? Constants.NullValueFor.String,
                    TeamMemberImageFilename = teamMemberImageFilename,
                    TeamMemberIsPublished = viewModel.TeamMemberIsPublished,
                    TeamMemberCategoryID = viewModel.TeamMemberCategoryID
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasTeamMemberImage)
                {
                    await FileStorage.SaveUploadedFile(
                        sourceFileStream: viewModel.TeamMemberImageFile.OpenReadStream(),
                        filename: teamMemberImageFilename
                    );
                }
            }
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
