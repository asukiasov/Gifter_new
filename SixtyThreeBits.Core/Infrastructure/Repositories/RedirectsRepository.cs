using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Infrastructure.Base;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Database.Core;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RedirectsRepository : RepositoryBase
    {
        #region Constructors
        public RedirectsRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.RedirectsListEntity, RedirectDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<int?> RedirectsIUD(Enums.DatabaseActions databaseAction, int? redirectID = null, string redirectFrom = null, string redirectTo = null)
        {
            return await TryToReturnAsyncTask($"{nameof(RedirectsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(redirectID)} = {redirectID}, {nameof(redirectFrom)} = {redirectFrom}, {nameof(redirectTo)} = {redirectTo})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.RedirectsIUD(databaseAction, redirectID, redirectFrom, redirectTo);
                    return redirectID;
                }
            });
        }

        public async Task<List<RedirectDTO>> RedirectsList()
        {
            return await TryToReturnAsyncTask($"{nameof(RedirectsList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.RedirectsList().OrderByDescending(item => item.RedirectDateCreated).ToListAsync())?.Select(item => _mapper.Map<RedirectDTO>(item)).ToList();
                }
            });
        }
        #endregion
    }
}
