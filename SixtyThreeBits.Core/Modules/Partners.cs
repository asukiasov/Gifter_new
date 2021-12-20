using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class PartnersDataAccess : DataAccessBase
    {
        #region Constructors
        public PartnersDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {

        }
        #endregion

        #region Methods
        public async Task<Partner> GetSinglePartnerByID(int? PartnerID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSinglePartnerByID)}({nameof(PartnerID)} = {PartnerID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.PartnersGetSingleByID(PartnerID);
                    return Result?.DeserializeJsonTo<Partner>();
                }
            });
        }

        public async Task<List<DBCoreDataContext.PartnersListResultItem>> ListPartners()
        {
            return await TryToReturnAsyncTask($"{nameof(ListPartners)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.PartnersList().OrderByDescending(Item => Item.PartnerDateCreated).ToListAsync();
                }
            });
        }

        public async Task<int?> PartnersIUD(Enums.DatabaseActions DatabaseAction, int? PartnerID = null, string PartnerName = null, string PartnerNameEng = null, string PartnerNameRus = null, string PartnerShortDescription = null, string PartnerShortDescriptionEng = null, string PartnerShortDescriptionRus = null, string PartnerFullDescription = null, string PartnerFullDescriptionEng = null, string PartnerFullDescriptionRus = null, string PartnerWebSite = null, string PartnerImageFilename = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PartnersIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PartnerID)} = {PartnerID}, {nameof(PartnerName)} = {PartnerName}, {nameof(PartnerNameEng)} = {PartnerNameEng}, {nameof(PartnerNameRus)} = {PartnerNameRus}, {nameof(PartnerShortDescription)} = {PartnerShortDescription}, {nameof(PartnerShortDescriptionEng)} = {PartnerShortDescriptionEng}, {nameof(PartnerFullDescription)} = {PartnerFullDescription}, {nameof(PartnerFullDescriptionEng)} = {PartnerFullDescriptionEng}, {nameof(PartnerFullDescriptionRus)} = {PartnerFullDescriptionRus}, {nameof(PartnerWebSite)} = {PartnerWebSite}, {nameof(PartnerImageFilename)} = {PartnerImageFilename})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    PartnerID = await db.PartnersIUD(DatabaseAction, PartnerID, PartnerName, PartnerNameEng, PartnerNameRus, PartnerShortDescription, PartnerShortDescriptionEng, PartnerShortDescriptionRus, PartnerFullDescription, PartnerFullDescriptionEng, PartnerFullDescriptionRus, PartnerWebSite, PartnerImageFilename);
                    return PartnerID;
                }
            });
        }
        #endregion
    }

    public class Partner
    {
        #region Properties
        public int? PartnerID { get; set; }
        public string PartnerName { get; set; }
        public string PartnerNameEng { get; set; }
        public string PartnerNameRus { get; set; }
        public string PartnerShortDescription { get; set; }
        public string PartnerShortDescriptionEng { get; set; }
        public string PartnerShortDescriptionRus { get; set; }
        public string PartnerFullDescription { get; set; }
        public string PartnerFullDescriptionEng { get; set; }
        public string PartnerFullDescriptionRus { get; set; }
        public string PartnerWebSite { get; set; }
        public string PartnerImageFilename { get; set; }
        public DateTime? PartnerDateCreated { get; set; }
        #endregion
    }
}
