using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using System;

namespace Mnemox.Security.Utils
{
    public class TenantContextValidationFilter: ActionFilterAttribute
    {
        private const string UNAUTHORIZED_ERROR_RESPONSE_TEXT = "Parameter tenant id not found in the URL";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var values = context.RouteData.Values;

            if (values.Keys.Contains(UrlAndContextPropertiesNames.TENANT_ID_PARAMETER_NAME))
            {
                var tenantId = Convert.ToInt64(values[UrlAndContextPropertiesNames.TENANT_ID_PARAMETER_NAME]);

                ///////////////////////////////////////////////////////////////////////////
                //validate that user has access to this tenant
                ///////////////////////////////////////////////////////////////////////////

                //var user = (User)context.HttpContext.Items[UrlAndContextPropertiesNames.REQUEST_USER];
                //context.HttpContext.Items[UrlAndRequestsParameters.USER] = user;
            }
            else
            {
                context.Result = new UnauthorizedObjectResult(
                    new ResponseError { ErrorMessage = UNAUTHORIZED_ERROR_RESPONSE_TEXT, ErrorCode = MnemoxStatusCodes.UNAUTHORIZED.ToString()
                });
            }
        }
    }
}
