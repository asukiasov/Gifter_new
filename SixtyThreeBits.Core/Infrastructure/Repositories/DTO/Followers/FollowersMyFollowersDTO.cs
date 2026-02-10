using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record FollowersMyFollowersDTO
    {
        #region Properties
        public int? FollowerID { get; init; }
        public int? FollowerFollowingUserID { get; init; }
        public int? FollowerFollowedUserID { get; init; }
        public DateTime? FollowerDateCreated { get; init; }
        public string FollowingUserFullname { get; init; }
        public string FollowingUserEmail { get; init; }
        #endregion
    }
}
