
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
using Asp.Versioning.ApiExplorer;
using Serilog;

namespace MiniLibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .WriteTo.File("Logs/log.txt", rollingInterval : RollingInterval.Day)
                .CreateLogger();

            // user-secret JWT
            config.AddUserSecrets<Program>();
          
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
                var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach(var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "MiniLibraryManagementSystem",
                        Version = description.ApiVersion.ToString(),
                    });
                }

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

            builder.Host.UseSerilog();
            var app = builder.Build();

            app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            $"MiniLibraryManagementSystem {description.ApiVersion}");
                    }
                });

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

