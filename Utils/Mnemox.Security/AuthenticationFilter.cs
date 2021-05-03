using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mnemox.Account.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Api.Security.Utils
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private const string UNAUTHORIZED_ERROR_RESPONSE_TEXT = "Unauthorized request";

        private readonly ITokensManager _tokensManager;

        private readonly IMemoryCacheFacade _memoryCacheFacade;

        public AuthenticationFilter(ITokensManager tokensManager, IMemoryCacheFacade memoryCacheFacade)
        {
            _tokensManager = tokensManager;

            _memoryCacheFacade = memoryCacheFacade;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.HttpContext.Request.Headers.TryGetValue(UrlAndContextPropertiesNames.AUTHENTICATION_HEADER_NAME, out var token))
            {
                var tokenDetails = await _tokensManager.GetTokenDetails(token);

                if(tokenDetails == null)
                {
                    SetUnauthorized(context);

                    return;
                }

                context.HttpContext.Items.Add(UrlAndContextPropertiesNames.REQUEST_OWNER,
                        new RequestOwner
                        {
                            OwnerId = tokenDetails.OwnerId,
                            OwnerTypeId = tokenDetails.OwnerTypeId
                        }
                    );

                await next();
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
