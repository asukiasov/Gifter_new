using SixtyThreeBits.Core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SixtyThreeBits.Core.DB;
using System;

namespace SixtyThreeBits.Web
{
    public class Startup
    {
        AppSettingsModel AppSettings;

        public Startup(IWebHostEnvironment Env)
        {
            if (Env.IsDevelopment())
            {
                var Builder = new ConfigurationBuilder().SetBasePath(Env.ContentRootPath).AddJsonFile("appsettings.json");
                AppSettings = new AppSettingsModel(Builder.Build());
                AppSettings.IsDevelopment = true;
            }
            else
            {
                var Builder = new ConfigurationBuilder().SetBasePath(Env.ContentRootPath).AddJsonFile("appsettings.release.json");
                AppSettings = new AppSettingsModel(Builder.Build());
                AppSettings.IsDevelopment = false;
            }
        }

        public void ConfigureServices(IServiceCollection Services)
        {
            Services.AddSingleton(AppSettings);
            Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = AppSettings.IsDevelopment ? $"{Constants.ProjectName}Development" : $"{Constants.ProjectName}Production";
                options.Cookie.HttpOnly = true;                
            });
            Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.AddControllersWithViews().AddJsonOptions(Options => { Options.JsonSerializerOptions.PropertyNamingPolicy = null;  } ); ;
            Services.AddDbContext<DBCoreDataContext>(Options => Options.UseSqlServer(AppSettings.DBConnectionStrings.DBConnectionString));
            Services.Configure<RouteOptions>(routeOptions => {
                routeOptions.AppendTrailingSlash = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder App, IWebHostEnvironment Env)
        {
            if (AppSettings.IsDevelopment)
            {
                App.UseDeveloperExceptionPage();
            }

            App.UseFileServer();
            App.UseSession(new SessionOptions { IdleTimeout = TimeSpan.FromMinutes(60) });
            App.UseRouting();            
            
            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapControllers();                
            });
        }
    }
}
