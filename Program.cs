
using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.ExtensionMethods;
using MiniLibraryManagementSystem.Helper;
using MiniLibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static System.Net.WebRequestMethods;
using MiniLibraryManagementSystem.SeedData;

namespace MiniLibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            options.SignIn.RequireConfirmedEmail = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

           


            // user-secret JWT
            builder.Configuration.AddUserSecrets<Program>();

            builder.Services.AddAuthentication(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTSecretKey"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "https://localhost:7218",
                    ValidAudience = "https://localhost:7218",
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // dependency injection using Register groups of services with extension methods
            // حقيقي مبسوطة اني طبقت الجزئية دي بجد 
            builder.Services.AddConfig(config)
                .AddMyDependencyGroup();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"))
                .Options;
            ApplicationDbContext _context = new ApplicationDbContext(options);
            SeedingData  seedData = new SeedingData(_context);

            seedData.Seed();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "MiniLibraryManagementSystem", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "أدخل التوكن هنا بدون كلمة Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();
          

            app.MapControllers();

            app.Run();
        }
    }
}

