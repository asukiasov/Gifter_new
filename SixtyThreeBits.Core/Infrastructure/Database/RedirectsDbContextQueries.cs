using System;
using System.Linq;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region RedirectsList
        public record RedirectsListEntity
        (
            int? RedirectID,
            string RedirectFrom,
            string RedirectTo,
            DateTime? RedirectDateCreated
        );
        public IQueryable<RedirectsListEntity> RedirectsList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(RedirectsList)
            );
            var result = sqb.ExecuteQuery<RedirectsListEntity>();
            return result;
        }
        #endregion
    }
}