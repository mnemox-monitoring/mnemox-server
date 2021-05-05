using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mnemox.Account.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Utils;
using System;
using System.Threading.Tasks;

namespace Mnemox.Api.Security.Utils
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private const string UNAUTHORIZED_ERROR_RESPONSE_TEXT = "Unauthorized request";

        private readonly ITokensManager _tokensManager;

        private readonly IMemoryCacheFacade _memoryCacheFacade;

        private readonly ITenantObjectsManager _tenantObjectsManager;

        public AuthenticationFilter(
            ITokensManager tokensManager, 
            IMemoryCacheFacade memoryCacheFacade,
            ITenantObjectsManager tenantObjectsManager)
        {
            _tokensManager = tokensManager;

            _memoryCacheFacade = memoryCacheFacade;

            _tenantObjectsManager = tenantObjectsManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.HttpContext.Request.Headers.TryGetValue(UrlAndContextPropertiesNames.AUTHENTICATION_HEADER_NAME, out var token))
            {
                var requestOwner = _memoryCacheFacade.Get<RequestOwner>(token[0]);

                if (requestOwner == null)
                {
                    var tokenDetails = await _tokensManager.GetTokenDetailsFromDataStorgeAsync(token);

                    if (tokenDetails == null)
                    {
                        SetUnauthorized(context);

                        return;
                    }

                    var objectTenants = await _tenantObjectsManager.GetObjectTenantsAsync(
                        tokenDetails.OwnerId, 
                        tokenDetails.MnemoxAccessObjectsType);

                    if(objectTenants == null)
                    {
                        SetUnauthorized(context);

                        return;
                    }

                    requestOwner = new RequestOwner
                    {
                        OwnerId = tokenDetails.OwnerId,
                        MnemoxAccessObjectsType = tokenDetails.MnemoxAccessObjectsType,
                        OwnerTenants = objectTenants
                    };

                    var tokenTtlInMinutes = _tokensManager.GetTokenTtlMinutes(tokenDetails.ValidUntilDateTimeUtc.ToLocalTime());

                    _memoryCacheFacade.Set(tokenDetails.Token, requestOwner, TimeSpan.FromMinutes(tokenTtlInMinutes));
                }

                context.HttpContext.Items.Add(UrlAndContextPropertiesNames.REQUEST_OWNER, requestOwner);

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
