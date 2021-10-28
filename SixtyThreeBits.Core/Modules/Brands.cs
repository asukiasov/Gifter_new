using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class BrandsDataAccess : DataAccessBase
    {
        #region Constructors
        public BrandsDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {

        }
        #endregion

        #region Methods
        public async Task<int?> BrandsIUD(Enums.DatabaseActions DatabaseAction, int? BrandID = null, string BrandName = null, string BrandNameEng = null, string BrandNameRus = null, string BrandImageFilename = null)
        {
            return await TryToReturnAsyncTask($"{nameof(BrandsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(BrandID)} = {BrandID}, {nameof(BrandName)} = {BrandName}, {nameof(BrandNameEng)} = {BrandNameEng}, {nameof(BrandNameRus)} = {BrandNameRus})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    BrandID = await db.BrandsIUD(DatabaseAction, BrandID, BrandName, BrandNameEng, BrandNameRus, BrandImageFilename);
                    return BrandID;
                }
            });
        }

        public async Task<Brand> GetSingleBrandByID(int? BrandID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleBrandByID)}({nameof(BrandID)} = {BrandID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.BrandsGetSingleByID(BrandID);
                    return Result?.DeserializeJsonTo<Brand>();
                }
            });
        }

        public async Task<List<DBCoreDataContext.BrandsListResultItem>> ListBrands()
        {
            return await TryToReturnAsyncTask($"{nameof(ListBrands)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.BrandsList().OrderByDescending(Item => Item.BrandDateCreated).ToListAsync();
                }
            });
        }
        
        #endregion        
    }

    public class Brand
    {
        #region Properties
        public int? BrandID { get; set; }
        public string BrandName { get; set; }
        public string BrandNameEng { get; set; }
        public string BrandNameRus { get; set; }
        public string BrandImageFilename { get; set; }
        public DateTime? BrandDateCreated { get; set; } 
        #endregion
    }
}
