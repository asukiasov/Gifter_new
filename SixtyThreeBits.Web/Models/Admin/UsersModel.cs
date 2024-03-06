using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class UsersModel : ModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var viewModel = new PageViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Users.GridAdd);
            viewModel.Grid = new PageViewModel.GridModel();

            var repository = RepositoriesFactory.GetRolesRepository();
            viewModel.Grid.Roles = await repository.RolesListAsKeyValueTuple();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Users.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Users.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Users.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Users.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Users.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Users.GridDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetUsersRepository();

            var viewModel = (await repository.UsersList())
            ?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                UserID = Item.UserID,
                UserFirstname = Item.UserFirstname,
                UserLastname = Item.UserLastname,
                UserEmail = Item.UserEmail,
                RoleID = Item.RoleID,
                UserIsActive = Item.UserIsActive,
                UserDateCreated = Item.UserDateCreated,
                UrlUserProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Users.User.Properties, new { userID = Item.UserID }),
            })
            .ToList();

            return viewModel;
        }

        public async Task ValidateUserEmail(string userEmail, int? userID)
        {
            var repository = RepositoriesFactory.GetUsersRepository();
            var isUniq = await repository.UsersIsEmailUnique(userEmail, userID);
            if (!isUniq)
            {
                Form.AddError(Resources.ValidationUserEmailNotUniq);
            }
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? userID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetUsersRepository();

            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var dbItem = await repository.UsersGetSingleByID(userID);
                if (dbItem != null)
                {
                    await DeleteUploadedFile(dbItem.UserAvatarFilename, folderPath: null);
                }
            }

            await repository.UsersIUD(
                databaseAction: databaseAction,
                userID: userID,
                roleID: submitModel.RoleID,
                userEmail: submitModel.UserEmail,
                userPassword: submitModel.UserPassword,
                userFirstname: submitModel.UserFirstname,
                userLastname: submitModel.UserLastname,
                userIsActive: submitModel.UserIsActive
            );

            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> Roles { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.UserID));

                    grid
                    .ID("UsersGrid")
                    .OnInitialized("usersModel.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlUserProperties));
                        columns.AddFor(m => m.UserFirstname).Caption(Resources.TextFirstname).Width(150).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.UserLastname).Caption(Resources.TextLastname).Width(150);
                        columns.AddFor(m => m.UserEmail).Caption(Resources.TextEmail).Width(200).ValidationRules(options =>
                        {
                            options.AddRequired();
                            //Options.AddEmail();
                        });
                        columns.AddFor(m => m.UserPassword).Caption(Resources.TextPassword).Width(150);
                        columns.AddFor(m => m.RoleID).Caption(Resources.TextRole).Width(150).InitLookupColumn(data: Roles);
                        columns.AddFor(m => m.UserIsActive).Caption(Resources.TextActive).Width(80).InitCheckboxColumn();
                        columns.AddFor(m => m.UserDateCreated).Caption(Resources.TextDateCreated).DataType(GridColumnDataType.DateTime).Width(140).InitDateColumn(true).AllowEditing(false);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? UserID { get; set; }
                    public string UserFirstname { get; set; }
                    public string UserLastname { get; set; }
                    public string UserEmail { get; set; }
                    public string UserPassword { get; set; }
                    public int? RoleID { get; set; }
                    public bool? UserIsActive { get; set; }
                    public DateTime? UserDateCreated { get; set; }
                    public string UrlUserProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class UserModelBase : ModelBase
    {
        #region Properties
        public UserDTO dbItem { get; set; }
        #endregion
    }

    public class UserPropertiesModel : UserModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel(PageViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
                viewModel.UserIsActive = dbItem.UserIsActive;
                viewModel.UserEmail = dbItem.UserEmail;
                viewModel.UserFirstname = dbItem.UserFirstname;
                viewModel.UserLastname = dbItem.UserLastname;
                viewModel.UserBirthdate = dbItem.UserBirthdate;
                viewModel.UserPhoneNumberMobile = dbItem.UserPhoneNumberMobile;
            }

            var repository = RepositoriesFactory.GetRolesRepository();
            viewModel.Roles = await repository.RolesListAsKeyValueSelectedTuple(dbItem.RoleID);
            return viewModel;
        }

        public async Task ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(await Validation.ValidateEmail(
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.UserEmail)),
                userEmail: viewModel.UserEmail,
                validateRequired: true,
                validateUnique: true,
                validationPredicateReturnTrueWhenError: async () =>
                {
                    var repository = RepositoriesFactory.GetUsersRepository();
                    var isEmailUnique = await repository.UsersIsEmailUnique(viewModel.UserEmail, dbItem.UserID);
                    return !isEmailUnique;
                }
            ));
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.UserFirstname)), valueToValidate: viewModel.UserFirstname));
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.UserLastname)), valueToValidate: viewModel.UserLastname));
        }

        public async Task Save(PageViewModel viewModel)
        {
            var repository = RepositoriesFactory.GetUsersRepository();
            await repository.UsersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                userID: dbItem.UserID,
                roleID: viewModel.RoleID ?? Constants.NullValueFor.Numeric,
                userEmail: viewModel.UserEmail,
                userPassword: viewModel.UserPassword,
                userFirstname: viewModel.UserFirstname,
                userLastname: viewModel.UserLastname,
                userBirthdate: viewModel.UserBirthdate ?? Constants.NullValueFor.Date,
                userPhoneNumberMobile: viewModel.UserPhoneNumberMobile ?? Constants.NullValueFor.String,
                userPersonalNumber: null,
                userAvatarFilename: null,
                userIsActive: viewModel.UserIsActive
            );
            viewModel.IsSaved = !repository.IsError;
            viewModel.AddError(repository.ErrorMessage);
        }
        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties
            public string UserFirstname { get; set; }
            public string UserLastname { get; set; }
            public DateTime? UserBirthdate { get; set; }
            public string UserEmail { get; set; }
            public string UserPassword { get; set; }
            public string UserPhoneNumberMobile { get; set; }
            public bool UserIsActive { get; set; }
            public int? RoleID { get; set; }

            public List<KeyValueSelectedTuple<int?, string>> Roles { get; set; }
            public bool HasRoles => Roles?.Any() == true;

            public readonly string FormatDate = Constants.Formats.Date;

            public readonly string TextActive = Resources.TextActive;
            public readonly string TextEmail = Resources.TextEmail;
            public readonly string TextPassword = Resources.TextPassword;
            public readonly string TextFirstname = Resources.TextFirstname;
            public readonly string TextLastname = Resources.TextLastname;
            public readonly string TextBirthDate = Resources.TextBirthDate;
            public readonly string TextPhoneCell = Resources.TextPhoneCell;
            public readonly string TextRole = Resources.TextRole;
            #endregion
        }
        #endregion
    }
}
