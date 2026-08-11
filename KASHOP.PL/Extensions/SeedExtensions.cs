using KASHOP.DAL.Data;
using KASHOP.PL.Utils;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.PL.Extensions
{
    public static class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            //if we want to seed the data when the application starts, you can call the DataSeed method of each seed data class here
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeders = services.GetServices<ISeedData>();
                foreach (var seeder in seeders)
                {
                    await seeder.DataSeed();
                }
            }            
        }
    }
}
