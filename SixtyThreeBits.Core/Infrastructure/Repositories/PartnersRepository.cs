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
                cfg.CreateMap<DbContextQueries.PartnersListEntity, PartnersListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<PartnerDTO> PartnersGetSingleByID(int? partnerID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PartnersGetSingleByID)}({nameof(partnerID)} = {partnerID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.PartnersGetSingleByID(partnerID);
                        var result = resultJson?.DeserializeJsonTo<PartnerDTO>();
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
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        partnerID = await db.PartnersIUD(databaseAction, partnerID, partnerName, partnerNameEng, partnerShortDescription, partnerShortDescriptionEng, partnerFullDescription, partnerFullDescriptionEng, partnerWebSite, partnerImageFilename, partnerIsPublished);
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.PartnersList().OrderByDescending(item => item.PartnerDateCreated).ToListAsync())?.Select(item => _mapper.Map<PartnersListDTO>(item)).ToList();
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }    
}