using System.Linq;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
    {
        #region CountriesList
        public record CountriesListEntity
        (
            int? CountryID,
            string CountryName,
            string CountryNameEng,
            string CountryCode2,
            string CountryCode3
        );
        public IQueryable<CountriesListEntity> CountriesList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(CountriesList),
                itemType: typeof(CountriesListEntity)
            );
            var result = sqb.ExecuteQuery<CountriesListEntity>();
            return result;
        }
        #endregion
    }
}