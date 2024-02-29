using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class CountriesRepository : RepositoryBase
    {
        #region Contructors
        public CountriesRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        #endregion

        #region Methods
        public async Task<List<KeyValueSelectedTuple<int?, string>>> CountriesListAsSimpleKeyValue(int? SelectedCountryID = null)
        {
            var result = await TryToReturnAsyncTask($"{nameof(CountriesListAsSimpleKeyValue)}({nameof(SelectedCountryID)} = {SelectedCountryID})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextQueries())
                {
                    var result = (await db.CountriesList().OrderBy(item => item.CountryName).ToListAsync()).Select(item => new KeyValueSelectedTuple<int?, string>
                    {
                        Key = item.CountryID,
                        Value = item.CountryName,
                        IsSelected = item.CountryID == SelectedCountryID
                    }).ToList();
                    return result;
                }
            });
            return result;
        }
        #endregion
    }
}
