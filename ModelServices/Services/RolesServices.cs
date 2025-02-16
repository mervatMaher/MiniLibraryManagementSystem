using MiniLibraryManagementSystem.ModelServices.IServices;
using Microsoft.AspNetCore.Identity;

namespace MiniLibraryManagementSystem.ModelServices.Services
{
    public class RolesServices : IRolesServices
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesServices(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task AddRolesAsync()
        {
            var roles = new List<string>
            {
                "Admin",
                "Author",
                "Customer"
            };

            foreach (var role in roles)
            {
                var existRole = await _roleManager.FindByNameAsync(role);

                if (existRole == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));

                }
            }
        }
        //public Task AddToRoles()
        //{

        //}

    }
}
