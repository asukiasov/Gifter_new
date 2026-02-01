using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/setup")]
    public class SetupController : ControllerBase<ModelBase>
    {
        [HttpGet]
        [Route("seed-admin")]
        public async Task<IActionResult> SeedAdmin()
        {
            var repository = Model.RepositoriesFactory.CreateUsersRepository();
            
            // Check if admin already exists
            var existingUser = await repository.UsersGetSingleByEmailAndPassword("admin@gifter.com", "asdf");
            if (existingUser != null)
            {
                return Content("Admin user already exists.");
            }

            var adminUser = new UserIudDTO
            {
                UserEmail = "admin@gifter.com",
                UserPassword = "asdf",
                UserFirstname = "System",
                UserLastname = "Administrator",
                UserFullname = "System Administrator",
                UserIsActive = true,
                RoleID = 1 // Assuming 1 is Admin role.
            };

            var userId = await repository.UsersIUD(Enums.DatabaseActions.INSERT, null, adminUser);

            if (userId.HasValue)
            {
                return Content($"Admin user created successfully with ID: {userId.Value}. You can now login at /admin/login with admin@gifter.com / asdf");
            }
            else
            {
                return Content("Failed to create admin user. Check logs for details.");
            }
        }
    }
}
