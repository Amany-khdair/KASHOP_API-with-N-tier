using KASHOP.BLL.Common;
using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Repository;
using KASHOP.PL.Utils;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace KASHOP.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            //if theres more seed data classes, you can add them here
            //builder.Services.AddScoped<ISeedData, CategorySeedDataClass>();

            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddExceptionHandler<GlobalExceptionHandler>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IProductService, BLL.Services.ProductService>();
            Services.AddScoped<ICartService, CartService>();
            Services.AddScoped<ICheckoutService, BLL.Services.CheckoutService>();

            Services.AddScoped<IFileService, BLL.Services.FileService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();

            Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = Configuration["Stripe:Secretkey"];
            Services.AddProblemDetails();
            return Services;
        }
    }
}
