using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RolesRepository : RepositoryBase
    {
        #region Contructors
        public RolesRepository(ConnectionFactory connectionFactory) : base(connectionFactory) 
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.RolesListEntity, RoleDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<int?> RolesIUD(Enums.DatabaseActions databaseAction, int? roleID = null, string roleName = null, int? roleCode = null)
        {
            return await TryToReturnAsyncTask($"{nameof(RolesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(roleID)} = {roleID}, {nameof(roleName)} = {roleName}, {nameof(roleCode)} = {roleCode})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    roleID = await db.RolesIUD(databaseAction, roleID, roleName, roleCode);
                    return roleID;
                }
            });
        }

        public async Task<List<RoleDTO>> RolesList()
        {
            return await TryToReturnAsyncTask($"{nameof(RolesList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.RolesList().OrderBy(item => item.RoleCode).ToListAsync())?.Select(item => _mapper.Map<RoleDTO>(item)).ToList();
                }
            });
        }

        public async Task<List<KeyValueTuple<int?,string>>> RolesListAsKeyValueTuple(bool IsRoleCodeAsKey = false)
        {
            return await TryToReturnAsyncTask($"{nameof(RolesListAsKeyValueTuple)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.RolesList().OrderBy(item => item.RoleCode).ToListAsync())?.Select(item => new KeyValueTuple<int?, string>
                    {
                        Key = IsRoleCodeAsKey ? item.RoleCode : item.RoleID,
                        Value = item.RoleName
                    }).ToList();
                }
            });
        }

        public async Task<List<KeyValueSelectedTuple<int?, string>>> RolesListAsKeyValueSelectedTuple(int? SelectedValue, bool IsRoleCodeAsKey = false)
        {
            return await TryToReturnAsyncTask($"{nameof(RolesListAsKeyValueTuple)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.RolesList().OrderBy(item => item.RoleCode).ToListAsync())?.Select(item => new KeyValueSelectedTuple<int?, string>
                    {
                        Key = IsRoleCodeAsKey ? item.RoleCode : item.RoleID,
                        Value = item.RoleName,
                        IsSelected = (IsRoleCodeAsKey ? (item.RoleCode == SelectedValue) : (item.RoleID == SelectedValue))
                    }).ToList();
                }
            });
        }

        public async Task RolesPermissionsUpdate(int? roleID, List<int?> permissionIDs)
        {
            var PermissionIDsJson = permissionIDs.ToJson();
            await TryExecuteAsyncTask($"{nameof(RolesPermissionsUpdate)}({nameof(roleID)} = {roleID}, {nameof(permissionIDs)} = {PermissionIDsJson})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.RolesPermissionsUpdate(roleID, PermissionIDsJson);
                }
            });
        }
        #endregion
    }    
}