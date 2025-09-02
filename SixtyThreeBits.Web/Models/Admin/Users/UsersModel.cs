using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class UsersModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.UsersController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();

            var repository = RepositoriesFactory.CreateRolesRepository();
            viewModel.Grid.Roles = await repository.RolesListAsKeyValueTuple();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.UsersController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.UsersController.GridDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateUsersRepository();

            var users = await repository.UsersList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : users.Select(Item => new ViewModel.GridModel.GridItem
            {
                UserID = Item.UserID,
                UserFirstname = Item.UserFirstname,
                UserLastname = Item.UserLastname,
                UserEmail = Item.UserEmail,
                RoleID = Item.RoleID,
                UserIsActive = Item.UserIsActive,
                UserDateCreated = Item.UserDateCreated,
                UrlUserProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.UserPropertiesController.Properties, new { userID = Item.UserID }),
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? userID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            var validationResult = await iudValidate(
                databaseAction: databaseAction, 
                userID: userID, 
                submitModel: submitModel
            );

            if (validationResult.HasErrors)
            {
                viewModel.Data = validationResult.ErrorMessage;
            }
            else
            {
                await iudProcessUserAvatar(databaseAction: databaseAction, userID: userID);

                var repository = RepositoriesFactory.CreateUsersRepository();
                await repository.UsersIUD(
                    databaseAction: databaseAction,
                    userID: userID,
                    user: new UserIudDTO
                    {
                        RoleID = submitModel.RoleID ?? Constants.NullValueFor.Numeric,
                        UserEmail = submitModel.UserEmail,
                        UserPassword = submitModel.UserPassword,
                        UserFirstname = submitModel.UserFirstname,
                        UserLastname = submitModel.UserLastname,
                        UserIsActive = submitModel.UserIsActive
                    }
                );
                viewModel.IsSuccess = !repository.IsError;
                viewModel.Data = repository.ErrorMessage;                
            }

            return viewModel;
        }
        async Task<ValidationResult63> iudValidate(Enums.DatabaseActions databaseAction, int? userID, ViewModel.GridModel.GridItem submitModel)
        {
            var result = new ValidationResult63();
            var error = default(Error63);
            if (databaseAction is Enums.DatabaseActions.INSERT or Enums.DatabaseActions.UPDATE)
            {
                error = await Validation63.ValidateEmail(
                    errorKey: null,
                    userEmail: submitModel.UserEmail,
                    validateRequired: true,
                    validateUnique: true,
                    validationPredicateReturnTrueWhenError: async () =>
                    {
                        var repository = RepositoriesFactory.CreateUsersRepository();
                        var isEmailUnique = await repository.UsersIsEmailUnique(submitModel.UserEmail, userID);
                        return !isEmailUnique;
                    }
                );
                result.AddError(error);
            }
            return result;
        }
        async Task iudProcessUserAvatar(Enums.DatabaseActions databaseAction, int? userID)
        {
            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var repository = RepositoriesFactory.CreateUsersRepository();
                var dbItem = await repository.UsersGetSingleByID(userID);
                if (dbItem != null)
                {
                    await FileStorage.DeleteFile(dbItem.UserAvatarFilename);
                }
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> Roles { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.UserID));

                    grid
                    .ID("UsersGrid")
                    .OnInitialized("model.onGridInit")
                    .OnRowUpdating("model.onGridRowUpdating")
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
                        columns.AddFor(m => m.RoleID).Caption(Resources.TextRole).Width(150).InitLookupColumn(data: Roles, allowNull: true);
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
}
