using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.PL.Utils
{
    public class RoleSeedData : ISeedData
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeedData(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }        
        public async Task DataSeed()
        {
            string[] roles = { "SuperAdmin", "Admin", "User" };
            if(!await _roleManager.Roles.AnyAsync()) 
            {
                foreach(var role in roles)
                {                  
                        await _roleManager.CreateAsync(new IdentityRole(role));                    
                }
            }
        }
    }
}
