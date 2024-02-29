using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region PartnersGetSingleByID        
        public async Task<string> PartnersGetSingleByID(int? partnerID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PartnersGetSingleByID),
                sqlParameters:
                [
                    partnerID.ToSqlParameter(nameof(partnerID),SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region PartnersList
        public record PartnersListEntity
        (
            int? PartnerID,
            string PartnerName,
            string PartnerNameEng,
            string PartnerWebSite,
            bool PartnerIsPublished,
            DateTime? PartnerDateCreated
        );
        public IQueryable<PartnersListEntity> PartnersList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PartnersList)
            );
            var result = sqb.ExecuteQuery<PartnersListEntity>();
            return result;
        }
        #endregion
    }
}