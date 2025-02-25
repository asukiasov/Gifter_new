using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class CountriesRepository : RepositoryBase
    {
        #region Contructors
        public CountriesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<List<CountryDTO>> CountriesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(CountriesList)}()",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(CountriesList)
                        );
                        var resultQueryable = sqb.ExecuteTableValuedFunction<CountryDTO>();
                        var result = await resultQueryable.ToListAsync();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<KeyValueSelectedTuple<int?, string>>> CountriesListAsSimpleKeyValue(int? SelectedCountryID = null)
        {
            var result = (await CountriesList())
                ?.Select(item => new KeyValueSelectedTuple<int?, string>
                {
                    Key = item.CountryID,
                    Value = item.CountryName,
                    IsSelected = item.CountryID == SelectedCountryID
                }).ToList();
            
            return result;
        }
        #endregion
    }
}
