using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ShoppingListServer.Database;
using ShoppingListServer.Helpers;
using ShoppingListServer.Services;

namespace ShoppingListServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IShoppingService, ShoppingService>();

            // MySql database
            // Pomelo.EntityFrameworkCore.MySql: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
            // A MySql database must be setup in the system with a name and user access specified in appsettings.json
            string connectionString = "server=" + appSettings.DbServerAddress + ";" +
                "user=" + appSettings.DbUser + ";" +
                "password=" + appSettings.DbPassword + ";" +
                "database=" + appSettings.DbName + ";";
            services.AddDbContextPool<AppDb>(
                options => options
                    .UseMySql(
                        connectionString,
                        new MySqlServerVersion(new Version(8, 0, 23)),
                        mysqlOptions =>
                        {
                            mysqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend);
                            //mysqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
                        })
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
