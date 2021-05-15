using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Mnemox.Logs.Models;
using Mnemox.Logs.Models.FilesLogs;
using Mnemox.Monitoring.Models;
using Mnemox.Logs.Utils.FileLogs;
using Mnemox.Timescale.DM.Dal;
using Mnemox.Timescale.DM.HeartBeat;
using Mnemox.Timescale.Models;
using System.IO;
using Mnemox.Api.Security.Utils;
using Mnemox.Shared.Utils;
using Mnemox.Account.Models;
using Mnemox.Timescale.DM.Account;
using Mnemox.Timescale.DM;
using Mnemox.Security.Utils;
using Mnemox.Timescale.DM.Tenants;
using Mnemox.Shared.Models.Settings;
using Mnemox.Web.Utils;
using Mnemox.Resources.Models;
using Mnemox.Timescale.DM.Resources;

namespace Mnemox.Monitoring.Server
{
    public class Startup
    {
        #region consts

        private const string SWAGGER_TITLE = "Mnemox Monitoring Server";
        private const string SWAGGER_DOCUMENTATION_FILE = "Mnemox.Monitoring.Server.xml";
        private const string SWAGGER_VERSION = "v1";
        private const string SWAGGER_JSON = "/swagger/v1/swagger.json";

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

            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            services.AddTransient<IServerSettings>(c => new ServerSettings
            {
                BasePath = applicationBasePath
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SWAGGER_VERSION, new OpenApiInfo { Title = SWAGGER_TITLE, Version = SWAGGER_VERSION });

                c.OperationFilter<SwaggerAuthorizationHeaders>();

                var filePath = Path.Combine(applicationBasePath, SWAGGER_DOCUMENTATION_FILE);

                c.IncludeXmlComments(filePath);

                c.EnableAnnotations();
            });

            var filesLogsManager = new FilesLogsManager(new FilesLogsConfiguration { });

            services.AddTransient<ILogsManager>(s => filesLogsManager);

            services.AddTransient<IMemoryCacheFacade, MemoryCacheFacade>();

            services.AddTransient<ISecretsManager, SecretsManager>();

            services.AddTransient<IWebFilesManagerHelpers, WebFilesManagerHelpers>();

            services.AddTransient<IWebFilesManager, WebFilesManager>();
            
            services.AddTransient<AuthenticationFilter>();

            services.AddTransient<TenantContextValidationFilter>();
            
            SetTimescaleDataManagerTs(services);
        }

        private void SetTimescaleDataManagerTs(IServiceCollection services)
        {
            const string TIMESCALEDB_FACTORY_SETTINGS_SECTION_NAME = "TimescaleDbFactorySettings";

            var dbFactorySettings = new DbFactorySettings();

            Configuration.GetSection(TIMESCALEDB_FACTORY_SETTINGS_SECTION_NAME).Bind(dbFactorySettings);

            services.AddTransient<IDbFactory>(c => new TimescaleDbFactory(dbFactorySettings));

            services.AddTransient<IHeartBeatDataManager, HeartBeatDataManagerTs>();

            services.AddTransient<IResourceDataManager, ResourcesDataManagerTs>();

            services.AddTransient<IUsersDataManagerTsHelpers, UsersDataManagerTsHelpers>();

            services.AddTransient<IUsersDataManager, UsersDataManagerTs>();

            services.AddTransient<IDataManagersHelpersTs, DataManagersHelpersTs>();

            services.AddTransient<IUsersDataManager, UsersDataManagerTs>();

            services.AddTransient<ITokensManager, TokensManagerTs>();

            services.AddTransient<ITenantObjectsManager, TenantsObjectsManagetTs>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();

            app.UseSwagger();

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
