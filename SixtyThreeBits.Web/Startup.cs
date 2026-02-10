using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Loggers;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace SixtyThreeBits.Web
{
    public class Startup
    {
        readonly IConfiguration _configuration;
        readonly AppSettingsCollection _appSettings;
        readonly UtilityCollection _utilities;
        readonly RepositoryFactory _repositoryFactory;
        readonly ILogger _logger = new ErrorLogTxtFileLogger();
        readonly bool _isDevelopmentEnvironment;

        public Startup(IWebHostEnvironment env)
        {            
            _isDevelopmentEnvironment = env.IsDevelopment();
            if (_isDevelopmentEnvironment)
            {
                _configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json").Build();
            }
            else
            {
                #if DEBUG
                _configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.debug.json").Build();
                #else
                _configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.production.json").Build();                
                #endif
            }

            _appSettings = new AppSettingsCollection(
                contentRootPath: env.ContentRootPath,
                webRootPath: env.WebRootPath,
                configuration: _configuration,
                isDevelopmentEnvironment: _isDevelopmentEnvironment
            );
            _utilities = new UtilityCollection(
                contentRootPath: env.ContentRootPath,
                webRootPath: env.WebRootPath
            );            
            _repositoryFactory = new RepositoryFactory(
                dbConnectionString: _appSettings.ConnectionStrings.DbConnectionString, 
                logger: _logger
            );
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

            // Authentication - Google Only
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            })
            .AddGoogle(options =>
            {
                options.ClientId = _configuration["Authentication:Google:ClientId"];
                options.ClientSecret = _configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google";

                // Request additional scopes for profile picture
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // Map the picture claim
                options.ClaimActions.MapJsonKey("picture", "picture");
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

            if (_isDevelopmentEnvironment)
            {
				app.UseDeveloperExceptionPage();
			}
            else
            {				
				app.UseExceptionHandler(exceptionHandlerApp =>
				{
					exceptionHandlerApp.Run(async context =>
					{
						var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
						if (exceptionHandlerPathFeature != null)
						{
							var messageCollected = await ExceptionRequestInformationCollector.Create(request: context.Request, exception: exceptionHandlerPathFeature.Error).Collect();
                            if (_logger != null)
                            {
                                _logger.LogError(exceptionHandlerPathFeature.Error, messageCollected);
                            }

                            await RenderNotFoundView(context);
						}
					});
				});

				app.UseHsts();
                
                urlRewriteOptions.AddRedirectToNonWwwPermanent().AddRedirectToHttpsPermanent();

                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
                    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; img-src *; style-src 'unsafe-inline' *; font-src *; script-src 'unsafe-inline' *");
                    await next();
                });
            }

            app.UseRewriter(urlRewriteOptions);

            app.UseFileServer();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(_appSettings.UploadFolderPhysicalPath),
                RequestPath = _appSettings.UploadFolderHttpPath.TrimEnd('/')
            });


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
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

        public async Task RenderNotFoundView(HttpContext context)
        {
			// Set the response content type to HTML
			context.Response.ContentType = "text/html";

			// Get the MVC services from the request scope
			var services = context.RequestServices;
			var viewEngine = services.GetRequiredService<Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine>();
			var tempDataFactory = services.GetRequiredService<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory>();
			var tempData = tempDataFactory.GetTempData(context);

			// Create an ActionContext using the current HttpContext
			var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
				context,
				new Microsoft.AspNetCore.Routing.RouteData(),
				new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
			);

			// Define the error view you want to return (e.g., Error.cshtml)
			var viewName = ViewNames.Website.Errors.NotFoundView; // You can define a specific view here

			// Use a ViewDataDictionary to pass data to the view (without ModelState)
			var viewModel = new NotFoundViewModel
			{
				PluginsClient = new PluginsClientViewModel(),
				UrlLogout = "/"
			};
			viewModel.PluginsClient.EnableBootstrap(true);
			var viewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
				new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
				new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()
			)
			{
				Model = viewModel
			};

			// Find the view
			var viewEngineResult = viewEngine.GetView(viewName, viewName, false);

			if (viewEngineResult.Success)
			{
				// Render the view and write it to the response stream
				var view = viewEngineResult.View;
				using (var writer = new System.IO.StringWriter())
				{
					var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(
						actionContext,
						view,
						viewData,
						tempData,
						writer,
						new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
					);

					await view.RenderAsync(viewContext);
					await context.Response.WriteAsync(writer.ToString());
				}
			}
			else
			{
				// If the view is not found, you can display a default message
				await context.Response.WriteAsync("<h1>An error occurred</h1>");
			}
		}
    }
}