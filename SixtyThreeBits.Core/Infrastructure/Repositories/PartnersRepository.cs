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
    public class PartnersRepository : RepositoryBase
    {
        #region Constructors
        public PartnersRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.PartnersListEntity, PartnersListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<PartnerDTO> PartnersGetSingleByID(int? partnerID)
        {
            return await TryToReturnAsyncTask($"{nameof(PartnersGetSingleByID)}({nameof(partnerID)} = {partnerID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.PartnersGetSingleByID(partnerID);
                    return Result?.DeserializeJsonTo<PartnerDTO>();
                }
            });
        }

        public async Task<int?> PartnersIUD(Enums.DatabaseActions databaseAction, int? partnerID = null, string partnerName = null, string partnerNameEng = null, string partnerShortDescription = null, string partnerShortDescriptionEng = null, string partnerFullDescription = null, string partnerFullDescriptionEng = null, string partnerWebSite = null, string partnerImageFilename = null, bool? partnerIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PartnersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(partnerID)} = {partnerID}, {nameof(partnerName)} = {partnerName}, {nameof(partnerNameEng)} = {partnerNameEng}, {nameof(partnerShortDescription)} = {partnerShortDescription}, {nameof(partnerShortDescriptionEng)} = {partnerShortDescriptionEng}, {nameof(partnerFullDescription)} = {partnerFullDescription}, {nameof(partnerFullDescriptionEng)} = {partnerFullDescriptionEng}, {nameof(partnerWebSite)} = {partnerWebSite}, {nameof(partnerImageFilename)} = {partnerImageFilename}, {nameof(partnerIsPublished)} = {partnerIsPublished})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    partnerID = await db.PartnersIUD(databaseAction, partnerID, partnerName, partnerNameEng, partnerShortDescription, partnerShortDescriptionEng, partnerFullDescription, partnerFullDescriptionEng, partnerWebSite, partnerImageFilename, partnerIsPublished);
                    return partnerID;
                }
            });
        }

        public async Task<List<PartnersListDTO>> PartnersList()
        {
            return await TryToReturnAsyncTask($"{nameof(PartnersList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.PartnersList().OrderByDescending(item => item.PartnerDateCreated).ToListAsync())?.Select(item => _mapper.Map<PartnersListDTO>(item)).ToList();
                }
            });
        }
        #endregion
    }    
}
