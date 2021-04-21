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

namespace Mnemox.Monitoring.Server
{
    public class Startup
    {
        #region consts

        private const string SWAGGER_TITLE = "Mnemox Monitoring Server";
        private const string SWAGGER_DOCUMENTATION_FILE = "Mnemox.Monitoring.Server.xml";
        private const string SWAGGER_VERSION = "v1";
        private const string SWAGGER_JSON = "/swagger/v1/swagger.json";
        private const string DEFAULT_RESPONSE_CONTENT_TYPE = "application/json";

        #endregion

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SWAGGER_VERSION, new OpenApiInfo { Title = SWAGGER_TITLE, Version = SWAGGER_VERSION });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, SWAGGER_DOCUMENTATION_FILE);

                c.IncludeXmlComments(filePath);

                c.EnableAnnotations();
            });

            var filesLogsManager = new FilesLogsManager(new FilesLogsConfiguration { });

            services.AddTransient<ILogsManager>(s => filesLogsManager);

            SetHeartBeatDataManagerTs(services);
        }

        private void SetHeartBeatDataManagerTs(IServiceCollection services)
        {
            const string TIMESCALEDB_FACTORY_SETTINGS_SECTION_NAME = "TimescaleDbFactorySettings";

            var dbFactorySettings = new DbFactorySettings();

            Configuration.GetSection(TIMESCALEDB_FACTORY_SETTINGS_SECTION_NAME).Bind(dbFactorySettings);

            services.AddTransient<IDbFactory>(c => new TimescaleDbFactory(dbFactorySettings));

            services.AddTransient<IHeartBeatDataManager, HeartBeatDataManagerTs>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint(SWAGGER_JSON, $"{SWAGGER_TITLE} {SWAGGER_VERSION}"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,

                DefaultContentType = DEFAULT_RESPONSE_CONTENT_TYPE
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
