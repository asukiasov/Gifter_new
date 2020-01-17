using SixtyThreeBits.Core.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties
        public RolesDataAccess Roles { get; set; }
        public UsersDataAccess Users { get; set; }
        #endregion

        #region Constructors
        public DataAccessFactory(DBCoreDataContext db)
        {
            Roles = new RolesDataAccess(db);
            Users = new UsersDataAccess(db);
        } 
        #endregion
    }
}
