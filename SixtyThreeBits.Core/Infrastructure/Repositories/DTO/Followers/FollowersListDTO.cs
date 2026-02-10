using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record FollowersListDTO
    {
        #region Properties
        public int? FollowerID { get; init; }
        public int? FollowerFollowingUserID { get; init; }
        public int? FollowerFollowedUserID { get; init; }
        public DateTime? FollowerDateCreated { get; init; }
        public string FollowedUserFullname { get; init; }
        public string FollowedUserEmail { get; init; }
        #endregion
    }
}
