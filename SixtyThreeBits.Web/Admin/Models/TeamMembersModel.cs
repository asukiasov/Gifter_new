using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class TeamMembersModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridDelete);
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridDelete);
            ViewModel.Grid.TeamMemberCategories = await DataAccessFactory.Dictionaries.ListDictionariesAsSimpleKeyValue(Enums.DictionaryCodes.TeamMemberCategories);
            ViewModel.UrlSync = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersSyncSortIndexes);
            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var TeamMembers = (await DataAccessFactory.TeamMembers.ListTeamMembers())
            ?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                TeamMemberID = Item.TeamMemberID,
                TeamMemberFirstname = Item.TeamMemberFirstname,
                TeamMemberLastname = Item.TeamMemberLastname,
                TeamMemberPosition = Item.TeamMemberPosition,
                TeamMemberIsPublished = Item.TeamMemberIsPublished,
                TeamMemberCategoryID = Item.TeamMemberCategoryID,
                TeamMemberSortIndex = Item.TeamMemberSortIndex,
                UrlTeamMemberProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMember.Properties, new { TeamMemberID = Item.TeamMemberID })
            }).OrderBy(Item=> Item.TeamMemberSortIndex).ToList();

            return TeamMembers;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? TeamMemberID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.TeamMembers.GetSingleTeamMemberID(TeamMemberID);
                if (DBItem != null)
                {
                    Utilities.DeleteUploadedFile(DBItem.TeamMemberImageFilename);
                }
            }
            await DataAccessFactory.TeamMembers.TeamMembersIUD(
                DatabaseAction: DatabaseAction,
                TeamMemberID: TeamMemberID,
                TeamMemberFirstname: SubmitModel.TeamMemberFirstname,
                TeamMemberLastName: SubmitModel.TeamMemberLastname,
                TeamMemberPosition: SubmitModel.TeamMemberPosition,
                TeamMemberIsPublished: SubmitModel.TeamMemberIsPublished,
                TeamMemberCategoryID: SubmitModel.TeamMemberCategoryID
            );

            if (DataAccessFactory.TeamMembers.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }

        public async Task<AjaxResponse> TeamMembersSyncSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.TeamMembers.TeamMembersSyncSortIndexes(SubmitModel.SortIndexes);

            AR.IsSuccess = !DataAccessFactory.TeamMembers.IsError;
            return AR;
        }

        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public GridModel Grid { get; set; }
            public string UrlSync { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Properties
                public List<SimpleKeyValue<int?, string>> TeamMemberCategories { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.TeamMemberID));

                    Grid
                   .Sorting(sorting => sorting.Mode(GridSortingMode.None))
                   .Pager(Options =>
                   {
                       Options.Visible(false);
                   })
                   .RowDragging(rd => rd
                        .AllowReordering(true)
                        .OnReorder("TeamMembersModel.OnGridReorder")
                        .DropFeedbackMode(DropFeedbackMode.Push)
                        .ShowDragIcons(true)
                    )
                   .Paging(Options =>
                   {
                       Options.Enabled(false);
                   })
                   .ID("TeamMembersGrid")
                   .FilterRow(Options =>
                   {
                       Options.Visible(false);
                   })
                   .OnInitialized("TeamMembersModel.OnGridInit")                   
                   .Columns(Columns =>
                   {
                       Columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlTeamMemberProperties));
                       Columns.AddFor(m => m.TeamMemberFirstname).Caption("სახელი").Width(150).ValidationRules(Options =>
                       {
                           Options.AddRequired();
                       });
                       Columns.AddFor(m => m.TeamMemberLastname).Caption("გვარი").Width(150).ValidationRules(Options =>
                       {
                           Options.AddRequired();
                       });
                       Columns.AddFor(m => m.TeamMemberPosition).Caption("პოზიცია").Width(150);
                       Columns.AddFor(m => m.TeamMemberCategoryID).Caption("კატეგორია").Width(150).InitLookupColumn(Data: TeamMemberCategories, AllowNull: true);
                       Columns.AddFor(m => m.TeamMemberIsPublished).Caption("გამოქვეყნებული").DataType(GridColumnDataType.Boolean).Width(130).InitCheckboxColumn();
                       Columns.Add();
                   });

                    return Grid;

                }
                #endregion

                #region Sub Classes
                public class GridItem
                {
                    #region Properties
                    public int? TeamMemberID { get; set; }
                    public string TeamMemberFirstname { get; set; }
                    public string TeamMemberLastname { get; set; }
                    public string TeamMemberPosition { get; set; }
                    public bool? TeamMemberIsPublished { get; set; }
                    public int? TeamMemberCategoryID { get; set; }
                    public int? TeamMemberSortIndex { get; set; }
                    public string UrlTeamMemberProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
    public class TeamMembersModelBase : WebProjectModelBase
    {
        #region Properties        
        public TeamMember DBItemTeamMember { get; set; }
        #endregion
    }

    public class TeamMemberPropertiesModel : TeamMembersModelBase
    {
        #region Methods

        public void ValidateTeamMemberPropertiesViewModel(TeamMembersPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.TeamMemberFirstname)), ValueToValidate:ViewModel.TeamMemberFirstname),
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.TeamMemberLastname)), ValueToValidate:ViewModel.TeamMemberLastname),
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task<TeamMembersPropertiesViewModel> GetTeamMembersPropertiesViewModel(TeamMembersPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new TeamMembersPropertiesViewModel();
                ViewModel.TeamMemberFirstname = DBItemTeamMember.TeamMemberFirstname;
                ViewModel.TeamMemberLastname = DBItemTeamMember.TeamMemberLastname;
                ViewModel.TeamMemberPosition = DBItemTeamMember.TeamMemberPosition;
                ViewModel.TeamMemberShortDescription = DBItemTeamMember.TeamMemberShortDescription;
                ViewModel.TeamMemberLongDescription = DBItemTeamMember.TeamMemberLongDescription;
                ViewModel.TeamMemberIsPublished = DBItemTeamMember.TeamMemberIsPublished;
                ViewModel.TeamMemberCategoryID = DBItemTeamMember.TeamMemberCategoryID;
            }
            ViewModel.TeamMemberCategories = await DataAccessFactory.Dictionaries.ListDictionariesAsSimpleKeyValue(DictionaryCode: Enums.DictionaryCodes.TeamMemberCategories, SelectedValue: ViewModel.TeamMemberCategoryID);
            ViewModel.TeamMemberImageFilename = DBItemTeamMember.TeamMemberImageFilename;
            ViewModel.TeamMemberImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItemTeamMember.TeamMemberImageFilename);
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMember.TeamMembersItemDeleteImage, new { TeamMemberID = DBItemTeamMember.TeamMemberID });
            return ViewModel;
        }

        public async Task SaveTeamMemberProperties(TeamMembersPropertiesViewModel ViewModel)
        {
            var HasTeamMemberImage = ViewModel.TeamMemberImageFile?.Length > 0;
            var TeamMemberImageFilename = HasTeamMemberImage ? GetFilenameFromUploadedFile(ViewModel.TeamMemberImageFile) : null;
            if (HasTeamMemberImage)
            {
                Utilities.DeleteUploadedFile(DBItemTeamMember.TeamMemberImageFilename);
            }

            await DataAccessFactory.TeamMembers.TeamMembersIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                TeamMemberID: DBItemTeamMember.TeamMemberID,
                TeamMemberFirstname: ViewModel.TeamMemberFirstname,
                TeamMemberLastName: ViewModel.TeamMemberLastname,
                TeamMemberPosition: ViewModel.TeamMemberPosition ?? Constants.NullValueFor.String,
                TeamMemberShortDescription: ViewModel.TeamMemberShortDescription ?? Constants.NullValueFor.String,
                TeamMemberLongDescription: ViewModel.TeamMemberLongDescription ?? Constants.NullValueFor.String,
                TeamMemberImageFilename: TeamMemberImageFilename,
                TeamMemberIsPublished: ViewModel.TeamMemberIsPublished,
                TeamMemberCategoryID: ViewModel.TeamMemberCategoryID
            );

            if (!DataAccessFactory.TeamMembers.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasTeamMemberImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.TeamMemberImageFile, Filename: TeamMemberImageFilename);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage(int? TeamMemberID)
        {
            var TeamMemberProperties = await DataAccessFactory.TeamMembers.GetSingleTeamMemberID(TeamMemberID);
            Utilities.DeleteUploadedFile(TeamMemberProperties.TeamMemberImageFilename);

            var AR = new AjaxResponse();
            await DataAccessFactory.TeamMembers.TeamMembersIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                TeamMemberID: TeamMemberID,
                TeamMemberImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.TeamMembers.IsError;

            return AR;
        }

        #endregion

        #region Sub Classes
        public class TeamMembersPropertiesViewModel : FormViewModelBase
        {
            #region Properties             
            public string TeamMemberFirstname { get; set; }
            public string TeamMemberLastname { get; set; }
            public string TeamMemberPosition { get; set; }
            public string TeamMemberShortDescription { get; set; }
            public string TeamMemberLongDescription { get; set; }
            public string TeamMemberImageFilename { get; set; }
            public bool TeamMemberIsPublished { get; set; }
            public int TeamMemberCategoryID { get; set; }
            public bool HasTeamMemberCategories => TeamMemberCategories?.Count > 0;
            public List<SimpleKeyValue<int?, string>> TeamMemberCategories { get; set; }
            public string TeamMemberImageHttpPath { get; set; }
            public bool HasTeamMemberImage => !string.IsNullOrWhiteSpace(TeamMemberImageFilename);
            public string UrlDeleteImage { get; set; }
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            public IFormFile TeamMemberImageFile { get; set; }
            #endregion
        }
        #endregion
    }
}
