using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.PL.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddControllers();
            Services.AddOpenApi();
            Services.AddDatabaseServices(Configuration);
            Services.AddIdentityServices();
            Services.AddJwtAuthenticationServices(Configuration);
            Services.AddLocalizationServices();
            Services.AddApplicationServices();

            return Services;
        }
    }
}

