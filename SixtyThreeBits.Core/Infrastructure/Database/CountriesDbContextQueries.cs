using System.Linq;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
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
                databaseObjectName: nameof(CountriesList)
            );
            var result = sqb.ExecuteQuery<CountriesListEntity>();
            return result;
        }
        #endregion
    }
}