using MiniLibraryManagementSystem.Helper;
using MiniLibraryManagementSystem.HelperServices.IServices;
using MiniLibraryManagementSystem.HelperServices.Services;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ModelServices.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MiniLibraryManagementSystem.ExtensionMethods
{
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services,
            IConfiguration config)
        {

            services.Configure<SendGridApiAuth>(config.GetSection("SendGridApiAuth"));
            services.Configure<JWT>(config.GetSection("JWT"));
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }
       
        public static IServiceCollection AddMyDependencyGroup (this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITokenServices, TokenServices>();

            services.AddScoped<IRolesServices, RolesServices>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<IBookServices, BookServices>();
            services.AddScoped<IFavoriteServices, FavoriteServices>();
            services.AddScoped<IUploadFilesServices, UploadFilesServices>();
            services.AddScoped<IConfirmationEmails, ConfirmationEmails>();  
            services.AddScoped<IAuthorServices, AuthorServices>();
            services.AddSingleton<IEmailSender, EmailSender>();

            

            return services;
        }
    }
}
