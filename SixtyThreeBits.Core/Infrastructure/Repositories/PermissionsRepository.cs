using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class PermissionsRepository : RepositoryBase
    {
        #region Contructors
        public PermissionsRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.PermissionsListEntity, PermissionDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task PermissionsDeleteRecursive(int? permissionID)
        {
            await TryExecuteAsyncTask($"{nameof(PermissionsDeleteRecursive)}({nameof(permissionID)} = {permissionID})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.PermissionsDeleteRecursive(permissionID);
                }
            });
        }

        public async Task<int?> PermissionsIUD(Enums.DatabaseActions databaseAction, int? permissionID = null, int? permissionParentID = null, string permissionCaption = null, string permissionCaptionEng = null, string permissionPagePath = null, string permissionCodeName = null, string permissionCode = null, int? permissionSortIndex = null, bool? permissionIsMenuItem = null, string permissionMenuIcon = null, string permissionMenuTitle = null, string permissionMenuTitleEng = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PermissionsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(permissionID)} = {permissionID}, {nameof(permissionParentID)} = {permissionParentID}, {nameof(permissionCaption)} = {permissionCaption}, {nameof(permissionCaptionEng)} = {permissionCaptionEng}, {nameof(permissionPagePath)} = {permissionPagePath}, {nameof(permissionCodeName)} = {permissionCodeName}, {nameof(permissionCode)} = {permissionCode}, {nameof(permissionSortIndex)} = {permissionSortIndex}, {nameof(permissionIsMenuItem)} = {permissionIsMenuItem}, {nameof(permissionMenuIcon)} = {permissionMenuIcon}, {nameof(permissionMenuTitle)} = {permissionMenuTitle}, {nameof(permissionMenuTitleEng)} = {permissionMenuTitleEng})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    permissionID = await db.PermissionsIUD(databaseAction, permissionID, permissionParentID, permissionCaption, permissionCaptionEng, permissionPagePath, permissionCodeName, permissionCode, permissionIsMenuItem, permissionMenuIcon, permissionMenuTitle, permissionMenuTitleEng, permissionSortIndex);
                    return permissionID;
                }
            });
        }

        public async Task<List<PermissionDTO>> PermissionsList()
        {
            return await TryToReturnAsyncTask($"{nameof(PermissionsList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.PermissionsList().OrderBy(P => P.PermissionSortIndex).ToListAsync())?.Select(item=>_mapper.Map<PermissionDTO>(item)).ToList();
                }
            });
        }

        public async Task<List<int?>> PermissionsListByRoleID(int? roleID)
        {
            return await TryToReturnAsyncTask($"{nameof(PermissionsListByRoleID)}({nameof(roleID)} = {roleID}", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return await db.PermissionsListByRoleID(roleID).Select(item => item.PermissionID).ToListAsync();
                }
            });
        }
        #endregion
    }        
}