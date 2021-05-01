using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mnemox.Account.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using System;

namespace Mnemox.Api.Security.Utils
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

                var requestOwner = (RequestOwner)context.HttpContext.Items[UrlAndContextPropertiesNames.REQUEST_OWNER];

                if (!requestOwner.OwnerTenants.Exists(t => t == tenantId))
                {
                    //SetUnauthorized(context);
                }
            }
            else
            {
                SetUnauthorized(context);
            }
        }

        private void SetUnauthorized(ActionExecutingContext context)
        {
            context.Result = new UnauthorizedObjectResult(
                new ResponseError
                {
                    ErrorMessage = UNAUTHORIZED_ERROR_RESPONSE_TEXT,
                    ErrorCode = MnemoxStatusCodes.UNAUTHORIZED.ToString()
                });
        }
    }
}
