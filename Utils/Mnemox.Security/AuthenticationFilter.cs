using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mnemox.Account.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using System.Collections.Generic;

namespace Mnemox.Api.Security.Utils
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private const string UNAUTHORIZED_ERROR_RESPONSE_TEXT = "Unauthorized request";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Headers.TryGetValue(UrlAndContextPropertiesNames.AUTHENTICATION_HEADER_NAME, out var token))
            {
                //TODO: validate token
                context.HttpContext.Items.Add(UrlAndContextPropertiesNames.REQUEST_OWNER,
                        new RequestOwner
                        {
                            OwnerId = 1,
                            OwnerTenants = new List<long>
                            {
                                1
                            },
                            OwnerType = MnemoxAccessObjectsTypes.AGENT
                        }
                    );
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
