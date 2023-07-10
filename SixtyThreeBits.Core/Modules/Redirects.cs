using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class RedirectsDataAccess : DataAccessBase
    {
        #region Constructors
        public RedirectsDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {
        }
        #endregion

        #region Methods
        public async Task<int?> RedirectsIUD(Enums.DatabaseActions DatabaseAction, int? RedirectID = null, string RedirectFrom = null, string RedirectTo = null)
        {
            return await TryToReturnAsyncTask($"{nameof(RedirectsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(RedirectID)} = {RedirectID}, {nameof(RedirectFrom)} = {RedirectFrom}, {nameof(RedirectTo)} = {RedirectTo})", async () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.RedirectsIUD(DatabaseAction, RedirectID, RedirectFrom, RedirectTo);
                    return RedirectID;
                }
            });
        }

        public async Task<List<DBCoreDataContext.RedirectsListResultItem>> RedirectsList()
        {
            return await TryToReturnAsyncTask($"{nameof(RedirectsList)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.RedirectsList().OrderByDescending(Item => Item.RedirectDateCreated).ToListAsync();
                }
            });
        }
        #endregion
    }
}
