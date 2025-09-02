using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record UsersListDTO
    {
        #region Properties
        public int? UserID { get; init; }
        public int? RoleID { get; init; }
        public string UserEmail { get; init; }
        public string UserPassword { get; init; }
        public string UserFirstname { get; init; }
        public string UserLastname { get; init; }
        public string UserFullname { get; init; }
        public DateTime? UserBirthdate { get; init; }
        public string UserPhoneNumberMobile { get; init; }
        public string UserPersonalNumber { get; init; }
        public string UserAvatarFilename { get; init; }
        public bool UserIsActive { get; init; }
        public DateTime? UserDateCreated { get; init; }
        #endregion
    }
}
