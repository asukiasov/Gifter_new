using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web
{
    public class Startup
    {
        readonly AppSettingsCollection _appSettings;
        readonly UtilityCollection _utilities;
        readonly RepositoryFactory _repositoryFactory;

        public Startup(IWebHostEnvironment env)
        {
            IConfiguration appSettingsConfiguration;
            if (env.IsDevelopment())
            {
                appSettingsConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json").Build();
            }
            else
            {
                #if DEBUG
                appSettingsConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.Staging.json").Build();
                #else
                appSettingsConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.Production.json").Build();                
                #endif
            }
            _appSettings = new AppSettingsCollection(env.WebRootPath, appSettingsConfiguration);
            _utilities = new UtilityCollection();
            _repositoryFactory = new RepositoryFactory(_appSettings.ConnectionStrings.CommandsConnectionString, _appSettings.ConnectionStrings.QueriesConnectionString);            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_appSettings);
            services.AddSingleton(_utilities);
            services.AddSingleton(_repositoryFactory);
            
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.Configure<CookiePolicyOptions>(Options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                Options.CheckConsentNeeded = context => false;
                Options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            

            services.AddControllersWithViews(Options=> { 
                Options.RespectBrowserAcceptHeader = true;                
            }).AddJsonOptions(options => { 
                options.JsonSerializerOptions.PropertyNamingPolicy = null;  
            });
            
            services.Configure<RouteOptions>(routeOptions => {
                routeOptions.AppendTrailingSlash = false;
            });

            //Response Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var urlRewriteOptions = new RewriteOptions().AddRedirect(@"(.*)/$", "$1", 301).AddRewrite(@"^$", "/", true).AddRewrite(@"(.*)/$", "$1", true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(Options =>
                {
                    app.UseExceptionHandler("/error/404/");
                });
                app.UseHsts();
                urlRewriteOptions.AddRedirectToNonWwwPermanent().AddRedirectToHttpsPermanent();                
            }

            app.UseRewriter(urlRewriteOptions);

            app.UseFileServer();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(_appSettings.UploadFolderPhysicalPath),
                RequestPath = _appSettings.UploadFolderHttpPath.TrimEnd('/')
            });
            app.UseRouting();
            app.UseSession();

            var requestLocalizationOptions = new RequestLocalizationOptions();
            requestLocalizationOptions.RequestCultureProviders.Clear();
            requestLocalizationOptions.RequestCultureProviders.Add(new CustomCultureProvider(_utilities));
            requestLocalizationOptions.SupportedCultures = _utilities.SupportedCultures;
            requestLocalizationOptions.SupportedUICultures = _utilities.SupportedCultures;
            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public class CustomCultureProvider : RequestCultureProvider
        {
            #region Properties
            readonly UtilityCollection _utilities;
            #endregion

            #region Constructors
            public CustomCultureProvider(UtilityCollection utilities)
            {
                _utilities = utilities;
            }
            #endregion

            #region Methods
            public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext context)
            {
                string culture;
                var path = context.Request.Path.ToString() ?? string.Empty;
                if (path.StartsWith("/admin/"))
                {
                    var languageCultureCode = context.Request.Cookies[WebConstants.Cookies.AdminLanguageCultureCode]?.ToString();
                    var language = _utilities.GetSupportedLanguageOrDefault(languageCultureCode);
                    culture = language.LanguageCultureCode;
                }
                else
                {
                    culture = context.Request.RouteValues[WebConstants.RouteValues.Culture]?.ToString() ?? _utilities.LanguageDefault.LanguageCultureCode;
                }

                await Task.Yield();
                return new ProviderCultureResult(culture);
            } 
            #endregion
        }
    }
}