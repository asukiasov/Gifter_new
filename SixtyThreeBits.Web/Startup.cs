using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Reusables;

namespace SixtyThreeBits.Web
{
    public class Startup
    {
        AppSettingsCollection AppSettings;
        UtilityCollection Utilities;

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
                var Builder = new ConfigurationBuilder().SetBasePath(Env.ContentRootPath).AddJsonFile("appsettings.release.json");
                AppSettings = new AppSettingsCollection(Builder.Build());
                AppSettings.IsDevelopment = false;
            }
            Utilities = new UtilityCollection(AppSettings);
        }

        public void ConfigureServices(IServiceCollection Services) 
        {            
            Services.AddSingleton(AppSettings);
            Services.AddSingleton(Utilities);

            Services.AddDistributedMemoryCache();
            Services.Configure<CookiePolicyOptions>(Options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                Options.CheckConsentNeeded = context => false;
                Options.MinimumSameSitePolicy = SameSiteMode.None;
            }); 
            Services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = AppSettings.IsDevelopment ? $".{Constants.ProjectName}Development" : $".{Constants.ProjectName}Production";
                options.Cookie.IsEssential = true;                
            });            

            Services.AddHttpContextAccessor();
            Services.AddScoped<ISessionAssistance, SessionAssistance>();


            Services.AddControllersWithViews(Options=> { 
                Options.RespectBrowserAcceptHeader = true;                
            }).AddJsonOptions(Options => { 
                Options.JsonSerializerOptions.PropertyNamingPolicy = null;  
            } );

            Services.AddScoped<DataAccessFactory>();
            //Honestly, I'm very pissed of on EFCore team!!! because of "A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe."
            //The whole idea of .NET Core + DI is create once use anywhere. I'm not able to use same DBDataContext to perform multiple db queries, so what is the point of DI then?            
            //Services.AddDbContext<DBCoreDataContext>(Options => Options.UseSqlServer(AppSettings.DBConnectionStrings.DBConnectionString));


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

            App.UseFileServer();
            App.UseSession();
            App.UseRouting();            
            
            App.UseEndpoints(Endpoints =>
            {
                Endpoints.MapControllers();                                
            });
        }
    }
}
