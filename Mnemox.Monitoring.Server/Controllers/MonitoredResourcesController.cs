using Microsoft.AspNetCore.Mvc;
using Mnemox.Api.Security.Utils;
using Mnemox.Logs.Models;
using Mnemox.Resources.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers
{
    [ServiceFilter(typeof(AuthenticationFilter))]
    [ServiceFilter(typeof(TenantContextValidationFilter))]
    [Route("tenant/{tenantId:long}/resources")]
    [ApiController]
    public class MonitoredResourcesController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        private readonly IResourceDataManager _resourcesDataManager;

        public MonitoredResourcesController(ILogsManager logsManager, IResourceDataManager resourcesDataManager)
        {
            _logsManager = logsManager;

            _resourcesDataManager = resourcesDataManager;
        }

        /// <summary>
        /// Adds a monitored resource
        /// </summary>
        /// <param name="resourcesBaseModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddResource([FromBody]ResourceBaseModel resourcesBaseModel)
        {
            try
            {
                var resourceId = await _resourcesDataManager.AddResource(resourcesBaseModel);

                return Ok(new ResourceIdModel { ResourceId = resourceId });
            }
            catch (OutputException ex)
            {
                return CreateErrorResultFromOutputException(ex);
            }
            catch (HandledException)
            {
                return InternalServerErrorResult();
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                return InternalServerErrorResult();
            }
        }
    }
}
