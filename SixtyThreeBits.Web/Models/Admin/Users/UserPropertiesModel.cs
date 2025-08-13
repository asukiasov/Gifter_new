using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class UserPropertiesModel : UserModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel(ViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.UserIsActive = dbItem.UserIsActive;
                viewModel.UserEmail = dbItem.UserEmail;
                viewModel.UserFirstname = dbItem.UserFirstname;
                viewModel.UserLastname = dbItem.UserLastname;
                viewModel.UserBirthdate = dbItem.UserBirthdate;
                viewModel.UserPhoneNumberMobile = dbItem.UserPhoneNumberMobile;
            }

            var repository = RepositoriesFactory.CreateRolesRepository();
            viewModel.Roles = await repository.RolesListAsKeyValueSelectedTuple(dbItem.RoleID);
            return viewModel;
        }

        public async Task ValidateViewModel(ViewModel viewModel)
        {
            viewModel.AddError(await Validation63.ValidateEmail(
                errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.UserEmail)),
                userEmail: viewModel.UserEmail,
                validateRequired: true,
                validateUnique: true,
                validationPredicateReturnTrueWhenError: async () =>
                {
                    var repository = RepositoriesFactory.CreateUsersRepository();
                    var isEmailUnique = await repository.UsersIsEmailUnique(viewModel.UserEmail, dbItem.UserID);
                    return !isEmailUnique;
                }
            ));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.UserFirstname)), valueToValidate: viewModel.UserFirstname));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.UserLastname)), valueToValidate: viewModel.UserLastname));
        }

        public async Task Save(ViewModel viewModel)
        {
            var repository = RepositoriesFactory.CreateUsersRepository();
            await repository.UsersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                userID: dbItem.UserID,
                user: new UserIudDTO
                {
                    RoleID = viewModel.RoleID ?? Constants.NullValueFor.Numeric,
                    UserEmail = viewModel.UserEmail,
                    UserPassword = viewModel.UserPassword,
                    UserFirstname = viewModel.UserFirstname,
                    UserLastname = viewModel.UserLastname,
                    UserBirthdate = Utilities.FormatDateSqlParseFriendly(viewModel.UserBirthdate ?? Constants.NullValueFor.Date),
                    UserPhoneNumberMobile = viewModel.UserPhoneNumberMobile ?? Constants.NullValueFor.String,
                    UserIsActive = viewModel.UserIsActive
                }

            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            
            viewModel.AddError(repository.ErrorMessage);
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
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
