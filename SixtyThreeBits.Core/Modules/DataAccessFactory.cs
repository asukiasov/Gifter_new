using SixtyThreeBits.Core.DB;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties
        public RolesDataAccess Roles { get; set; }
        public PermissionsDataAccess Permissions { get; set; }
        public UsersDataAccess Users { get; set; }
        public DictionariesDataAccess Dictionaries { get; set; }
        #endregion

        #region Constructors
        public DataAccessFactory(DBCoreDataContext db)
        {
            Roles = new RolesDataAccess(db);
            Permissions = new PermissionsDataAccess(db);
            Users = new UsersDataAccess(db);
            Dictionaries = new DictionariesDataAccess(db);
        } 
        #endregion
    }
}
