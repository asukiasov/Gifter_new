using Imageflow.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SixtyThreeBits.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web
{
    public class Startup
    {
        readonly AppSettingsCollection AppSettings;
        readonly UtilityCollection Utilities;

        public Startup(IWebHostEnvironment Env)
        {
            if (Env.IsDevelopment())
            {
                var Builder = new ConfigurationBuilder().SetBasePath(Env.ContentRootPath).AddJsonFile("appsettings.json");
                AppSettings = new AppSettingsCollection(Builder.Build());
                AppSettings.IsDevelopment = true;
            }
            else
            {
                #if DEBUG
                var Builder = new ConfigurationBuilder().SetBasePath(Env.ContentRootPath).AddJsonFile("appsettings.debug.json");
                AppSettings = new AppSettingsCollection(Builder.Build());
                AppSettings.IsDevelopment = true;
                #else
                var Builder = new ConfigurationBuilder().SetBasePath(Env.ContentRootPath).AddJsonFile("appsettings.release.json");
                AppSettings = new AppSettingsCollection(Builder.Build());
                AppSettings.IsDevelopment = false;
                #endif
            }
            Utilities = new UtilityCollection(AppSettings);
        }

        public void ConfigureServices(IServiceCollection Services)
        {
            Services.AddSingleton(AppSettings);
            Services.AddSingleton(Utilities);
            //Honestly, EFCore team are idiots !!! because of "A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe."
            //The whole idea of .NET Core + DI is create once use anywhere. I'm not able to use same DBDataContext to perform multiple db queries, so what is the point of DI then?
            //Services.AddDbContext<DBCoreDataContext>(Options => Options.UseSqlServer(AppSettings.DBConnectionStrings.DBConnectionString), optionsLifetime: ServiceLifetime.Scoped);
            
            Services.AddDistributedMemoryCache();
            Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                //options.Cookie.Name = AppSettings.IsDevelopment ? $".{Constants.ProjectName}Development" : $".{Constants.ProjectName}Production";
                options.Cookie.IsEssential = true;
            });
            Services.Configure<CookiePolicyOptions>(Options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                Options.CheckConsentNeeded = context => false;
                Options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            

            Services.AddControllersWithViews(Options=> { 
                Options.RespectBrowserAcceptHeader = true;                
            }).AddJsonOptions(Options => { 
                Options.JsonSerializerOptions.PropertyNamingPolicy = null;  
            });
            
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
            else
            {
                App.UseExceptionHandler(Options =>
                {
                    App.UseExceptionHandler("/error/404/");
                });
                App.UseHsts();
            }
            App.UseImageflow(new ImageflowMiddlewareOptions()
                .SetMapWebRoot(false)                
                .MapPath(AppSettings.UploadFolderVirtualPath, AppSettings.UploadFolderPhysicalPath));

            App.UseFileServer();
            App.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(AppSettings.UploadFolderPhysicalPath),
                RequestPath = $"/{AppSettings.UploadFolderVirtualName}"
            });
            App.UseRouting();
            App.UseSession();

            

            var RequestLocalizationOptions = new RequestLocalizationOptions();
            RequestLocalizationOptions.RequestCultureProviders.Clear();
            RequestLocalizationOptions.RequestCultureProviders.Add(new CustomCultureProvider(Utilities));
            RequestLocalizationOptions.SupportedCultures = new List<CultureInfo> { new CultureInfo(Enums.Languages.GEORGIAN) { NumberFormat = new NumberFormatInfo { CurrencyDecimalSeparator = "." } }, new CultureInfo(Enums.Languages.ENGLISH) };
            RequestLocalizationOptions.SupportedUICultures = new List<CultureInfo> { new CultureInfo(Enums.Languages.GEORGIAN) { NumberFormat = new NumberFormatInfo { CurrencyDecimalSeparator = "." } }, new CultureInfo(Enums.Languages.ENGLISH) };
            //App.UseRequestLocalization(RequestLocalizationOptions);

            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapControllers();
            });
        }

        public class CustomCultureProvider : RequestCultureProvider
        {
            UtilityCollection Utilities;

            public CustomCultureProvider(UtilityCollection Utilities)
            {
                this.Utilities = Utilities;
            }

            public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext Context)
            {
                string Culture = null;
                var Path = Context.Request.Path.ToString() ?? string.Empty;
                if (Path.StartsWith("/admin/"))
                {
                    Culture = Enums.Languages.GEORGIAN;
                }
                else
                {
                    Culture = Context.Request.RouteValues["Culture"]?.ToString() ?? Enums.Languages.GEORGIAN;
                }

                await Task.Yield();
                return new ProviderCultureResult(Culture);
            }
        }
    }
}
