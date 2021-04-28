using Microsoft.AspNetCore.Mvc;
using Mnemox.Api.Security.Utils;
using Mnemox.Components.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers
{
    [AuthenticationFilter]
    [TenantContextValidationFilter]
    [Route("tenant/{tenantId:long}/components")]
    [ApiController]
    public class MonitoredComponentsController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        private readonly IComponentsDataManager _componentsDataManager;

        public MonitoredComponentsController(ILogsManager logsManager, IComponentsDataManager componentsDataManager)
        {
            _logsManager = logsManager;

            _componentsDataManager = componentsDataManager;
        }

        /// <summary>
        /// Adds a component to the list of components
        /// </summary>
        /// <param name="componentBaseModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddComponent([FromBody]ComponentBaseModel componentBaseModel)
        {
            try
            {
                var componentId = await _componentsDataManager.AddComponent(componentBaseModel);

                return Ok(new ComponentIdModel { ComponentId = componentId });
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
