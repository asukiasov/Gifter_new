using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class PartnersRepository : RepositoryBase
    {
        #region Constructors
        public PartnersRepository(DbContextFactory connectionFactory) : base(connectionFactory)
        {
        }
        #endregion

        #region Methods
        public async Task<PartnerDTO> PartnersGetSingleByID(int? partnerID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PartnersGetSingleByID)}({nameof(partnerID)} = {partnerID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PartnersGetSingleByID),
                            sqlParameters:
                            [
                                partnerID.ToSqlParameter(nameof(partnerID),SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson.DeserializeJsonTo<PartnerDTO>();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> PartnersIUD(Enums.DatabaseActions databaseAction, int? partnerID = null, string partnerName = null, string partnerNameEng = null, string partnerShortDescription = null, string partnerShortDescriptionEng = null, string partnerFullDescription = null, string partnerFullDescriptionEng = null, string partnerWebSite = null, string partnerImageFilename = null, bool? partnerIsPublished = null)
        {
            partnerID = await TryToReturnAsyncTask(
                logString: $"{nameof(PartnersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(partnerID)} = {partnerID}, {nameof(partnerName)} = {partnerName}, {nameof(partnerNameEng)} = {partnerNameEng}, {nameof(partnerShortDescription)} = {partnerShortDescription}, {nameof(partnerShortDescriptionEng)} = {partnerShortDescriptionEng}, {nameof(partnerFullDescription)} = {partnerFullDescription}, {nameof(partnerFullDescriptionEng)} = {partnerFullDescriptionEng}, {nameof(partnerWebSite)} = {partnerWebSite}, {nameof(partnerImageFilename)} = {partnerImageFilename}, {nameof(partnerIsPublished)} = {partnerIsPublished})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PartnersIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                partnerID.ToSqlOutputParameter(nameof(partnerID),SqlDbType.Int),
                                partnerName.ToSqlParameter(nameof(partnerName),SqlDbType.NVarChar),
                                partnerNameEng.ToSqlParameter(nameof(partnerNameEng),SqlDbType.NVarChar),
                                partnerShortDescription.ToSqlParameter(nameof(partnerShortDescription),SqlDbType.NVarChar),
                                partnerShortDescriptionEng.ToSqlParameter(nameof(partnerShortDescriptionEng),SqlDbType.NVarChar),
                                partnerFullDescription.ToSqlParameter(nameof(partnerFullDescription),SqlDbType.NVarChar),
                                partnerFullDescriptionEng.ToSqlParameter(nameof(partnerFullDescriptionEng),SqlDbType.NVarChar),
                                partnerWebSite.ToSqlParameter(nameof(partnerWebSite),SqlDbType.NVarChar),
                                partnerImageFilename.ToSqlParameter(nameof(partnerImageFilename),SqlDbType.NVarChar),
                                partnerIsPublished.ToSqlParameter(nameof(partnerIsPublished),SqlDbType.Bit),
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        partnerID = sqb.GetNextOutputParameterValue<int?>();
                        return partnerID;
                    }
                }
            );
            return partnerID;
        }

        public async Task<List<PartnersListDTO>> PartnersList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PartnersList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PartnersList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<PartnersListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.PartnerDateCreated);
                        var result = await resultQueryable.ToListAsync();                        

                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }    
}