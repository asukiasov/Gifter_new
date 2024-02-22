using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class BrandsRepository : RepositoryBase
    {
        #region Constructors
        public BrandsRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbContextQueries.BrandsListEntity, BrandDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<BrandDTO> BrandsGetSingleByID(int? brandID)
        {
            return await TryToReturnAsyncTask($"{nameof(BrandsGetSingleByID)}({nameof(brandID)} = {brandID})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextQueries())
                {
                    var Result = await db.BrandsGetSingleByID(brandID);
                    return Result?.DeserializeJsonTo<BrandDTO>();
                }
            });
        }

        public async Task<int?> BrandsIUD(Enums.DatabaseActions databaseAction, int? brandID = null, string brandName = null, string brandNameEng = null, string brandImageFilename = null)
        {
            return await TryToReturnAsyncTask($"{nameof(BrandsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(brandID)} = {brandID}, {nameof(brandName)} = {brandName}, {nameof(brandNameEng)} = {brandNameEng})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextCommands())
                {
                    brandID = await db.BrandsIUD(databaseAction, brandID, brandName, brandNameEng, brandImageFilename);
                    return brandID;
                }
            });
        }

        public async Task<List<BrandDTO>> BrandsList()
        {
            return await TryToReturnAsyncTask($"{nameof(BrandsList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDbContextQueries())
                {
                    return (await db.BrandsList().OrderByDescending(item => item.BrandDateCreated).ToListAsync())?.Select(item => _mapper.Map<BrandDTO>(item)).ToList();
                }
            });
        }
        #endregion
    }        
}