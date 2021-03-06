﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Newtonsoft.Json;
using ShoppingListServer.Database;
using ShoppingListServer.Helpers;
using ShoppingListServer.Services;
using ShoppingListServer.LiveUpdates;
using ShoppingListServer.Services.Interfaces;

namespace ShoppingListServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static ServiceProvider _serviceProvider;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            // Replaced asp .net core 2.0 AddMvc()
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllersWithViews();

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
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                // API Key AUTH.
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is on update hub
                        // TO DO get rout from config
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/shoppingserver/update")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

           });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSignalR();

            // configure DI for application services
            // Scoped services are created once per request.
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IShoppingService, ShoppingService>();
            // Transient services are created each time they are requested.
            services.AddTransient<IShoppingHub, ShoppingHubService>();

            //_serviceProvider = services.BuildServiceProvider();


            // MySql database
            // Pomelo.EntityFrameworkCore.MySql: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
            // A MySql database must be setup in the system with a name and user access specified in appsettings.json

            string connectionString = "server=" + appSettings.DbServerAddress + ";" +
                "user=" + appSettings.DbUser + ";" +
                 "password=" + appSettings.DbPassword + ";" +
                 "database=" + appSettings.DbName + ";";

            ServerVersion service_version = ServerVersion.AutoDetect(connectionString);

            services.AddDbContextPool<AppDb>(
                options => options
                    .UseLazyLoadingProxies()
                    .UseMySql(
                        connectionString,
                        service_version,
                        mysqlOptions =>
                        {
                            // mysqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend);
                            // mysqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
                        })
                    .EnableSensitiveDataLogging()
#if DEBUG
                    .EnableDetailedErrors()
#endif
            );

            Console.WriteLine("Network adapter prioritization:");
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine(ni.NetworkInterfaceType.ToString());
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // All non handled exceptions in the controllers are handled by simply responding with the exceptions message.
            // https://stackoverflow.com/a/55166404
            // https://stackoverflow.com/a/38935583
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            // Handle all other non cached exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Unhandled_Exceptions);

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Server accessibility on browser routs
            app.UseStaticFiles();

#if DEBUG
            app.UseDeveloperExceptionPage();
#else
            app.UseHsts();
            app.UseHttpsRedirection();
#endif

            // SignalR/Websockets
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UpdateHub_Controller>("/shoppingserver/update");
                endpoints.MapControllers();
            });


        }

        private void Unhandled_Exceptions(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message, "Unhandled Exception");
        }
    }
}
