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
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(RedirectsList),
                itemType: typeof(RedirectsListEntity)
            );
            var result = sqb.ExecuteQuery<RedirectsListEntity>();
            return result;
        }
        #endregion
    }
}