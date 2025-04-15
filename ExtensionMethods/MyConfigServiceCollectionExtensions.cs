using MiniLibraryManagementSystem.Helper;
using MiniLibraryManagementSystem.HelperServices.IServices;
using MiniLibraryManagementSystem.HelperServices.Services;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ModelServices.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Asp.Versioning;
using MiniLibraryManagementSystem.Filters;

namespace MiniLibraryManagementSystem.ExtensionMethods
{
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services,
            IConfiguration config)
        {

            services.AddDefaultIdentity<ApplicationUser>(options =>
           options.SignIn.RequireConfirmedEmail = true)
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();


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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:JWTSecretKey"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "https://localhost:7218",
                    ValidAudience = "https://localhost:7218",
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddAuthorization();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader()
                    );

            })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
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
            services.AddScoped<JwtExceptionFilter>();
            

            return services;
        }
    }
}
