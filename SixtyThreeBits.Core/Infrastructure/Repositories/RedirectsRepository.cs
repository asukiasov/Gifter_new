using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
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
                cfg.CreateMap<DbContextQueries.RedirectsListEntity, RedirectDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<int?> RedirectsIUD(Enums.DatabaseActions databaseAction, int? redirectID = null, string redirectFrom = null, string redirectTo = null)
        {
            redirectID = await TryToReturnAsyncTask(
                logString: $"{nameof(RedirectsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(redirectID)} = {redirectID}, {nameof(redirectFrom)} = {redirectFrom}, {nameof(redirectTo)} = {redirectTo})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        redirectID = await db.RedirectsIUD(
                            databaseAction: databaseAction, 
                            redirectID: redirectID, 
                            redirectFrom: redirectFrom, 
                            redirectTo: redirectTo
                        );
                        return redirectID;
                    }
                }
            );
            return redirectID;
        }

        public async Task<List<RedirectDTO>> RedirectsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(RedirectsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (
                            await db.RedirectsList()
                            .OrderByDescending(item => item.RedirectDateCreated).ToListAsync()
                        )
                        ?.Select(item => _mapper.Map<RedirectDTO>(item))
                        .ToList();
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}
