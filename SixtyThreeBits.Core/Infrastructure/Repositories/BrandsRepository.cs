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
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsGetSingleByID)}({nameof(brandID)} = {brandID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.BrandsGetSingleByID(brandID: brandID);
                        var result = resultJson?.DeserializeJsonTo<BrandDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> BrandsIUD(Enums.DatabaseActions databaseAction, int? brandID = null, string brandName = null, string brandNameEng = null, string brandImageFilename = null)
        {
            brandID = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(brandID)} = {brandID}, {nameof(brandName)} = {brandName}, {nameof(brandNameEng)} = {brandNameEng})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        brandID = await db.BrandsIUD(
                            databaseAction: databaseAction, 
                            brandID: brandID, 
                            brandName: brandName, 
                            brandNameEng: brandNameEng, 
                            brandImageFilename: brandImageFilename
                        );
                        return brandID;
                    }
                }
            );
            return brandID;
        }

        public async Task<List<BrandDTO>> BrandsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (
                            await db.BrandsList()
                            .OrderByDescending(item => item.BrandDateCreated)
                            .ToListAsync()
                        )
                        ?.Select(item => _mapper.Map<BrandDTO>(item))
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