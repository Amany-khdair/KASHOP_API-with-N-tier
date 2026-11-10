using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace KASHOP.PL.Utils
{
    public class UserSeedData : ISeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserSeedData(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task DataSeed()
        {
            await CreateUserAsync("superAdmin@kashop.com", "SuperAdmin123!", "SuperAdmin", "Super Administrator", "SuperAdmin");
            await CreateUserAsync("admin@kashop.com", "Admin123!", "Admin", "Administrator", "Admin");
            await CreateUserAsync("user@kashop.com", "User123!", "User", "Regular User", "User");
        }       

        private async Task CreateUserAsync(string email, string password, string userName, string fullName, string role)
        {
            if (await _userManager.FindByEmailAsync(email) != null || await _userManager.FindByNameAsync(userName) != null) return; // User already exists            
            var user = new ApplicationUser
            {
                Email = email,
                UserName = userName,
                FullName = fullName,
                PhoneNumber = "1234567890",
                City = "Sample City",
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);            
            await _userManager.AddToRoleAsync(user, role);
            
        }       
    }
}
