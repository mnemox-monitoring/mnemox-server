using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Mnemox.Logs.Models;
using Mnemox.Logs.Models.FilesLogs;
using Mnemox.Logs.Utils.FileLogs;
using System.IO;
using Mnemox.Api.Security.Utils;
using Mnemox.Shared.Models.Settings;
using Mnemox.Web.Utils;
using Mnemox.Server.Utils;

namespace Mnemox.Monitoring.Server
{
    public class Startup
    {
        #region consts

        private const string SWAGGER_TITLE = "Mnemox Monitoring Server";
        private const string SWAGGER_DOCUMENTATION_FILE = "Mnemox.Monitoring.Server.xml";
        private const string SWAGGER_VERSION = "v1";
        private const string SWAGGER_JSON = "/swagger/v1/swagger.json";
        private const string SERVER_SETTINGS_SECTION_NAME = "ServerSettings";

        #endregion

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMemoryCache();

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()));

            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
            
            var fullSettings = new FullSettings();

            Configuration.Bind(fullSettings);

            fullSettings.ServerSettings.BasePath = applicationBasePath;

            services.AddSingleton<IFullSettings>(s => fullSettings);

            services.AddSingleton<ISettingsManager, SettingsManager>();

            services.AddTransient<IServerSettings>(s => fullSettings.ServerSettings);
            
            var apiConfiguration = new ApiConfiguration
            {
                Services = services,

                Configuration = Configuration
            };

            services.AddTransient<IApiConfiguration>(c => apiConfiguration);

            var filesLogsManager = new FilesLogsManager(new FilesLogsConfiguration { });

            services.AddTransient<ILogsManager>(s => filesLogsManager);

            services.AddTransient<IWebFilesManagerHelpers, WebFilesManagerHelpers>();

            services.AddTransient<IWebFilesManager, WebFilesManager>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SWAGGER_VERSION, new OpenApiInfo { Title = SWAGGER_TITLE, Version = SWAGGER_VERSION });

                c.OperationFilter<SwaggerAuthorizationHeaders>();

                var filePath = Path.Combine(applicationBasePath, SWAGGER_DOCUMENTATION_FILE);

                c.IncludeXmlComments(filePath);

                c.EnableAnnotations();
            });

            var serverInitializationManagerHelpers = new ServerInitializationManagerHelpers(fullSettings.ServerSettings, apiConfiguration);

            var serverInitialized = new ServerInitializationManager(
                fullSettings.ServerSettings, 
                filesLogsManager, 
                serverInitializationManagerHelpers, 
                apiConfiguration);

            serverInitialized.Initialize();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.Use((context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods",
                    new[] { "POST, GET, OPTIONS, PUT, DELETE" });
                return next();
            });

            app.UseSwaggerUI(c => c.SwaggerEndpoint(SWAGGER_JSON, $"{SWAGGER_TITLE} {SWAGGER_VERSION}"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
