
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Mnemox.Account.Models;
using Mnemox.Api.Security.Utils;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Utils;
using Moq;
using Shared;
using System;
using System.Collections.Generic;


namespace Auth.Tests
{
    public class AuthenticationFilterTestsHelpers
    {
        public static AuthenticationFilter GetTarget(
            Mock<ITokensManager> tokensManagerMock = null, 
            Mock<IMemoryCacheFacade> memoryCacheFacedeMock = null,
            Mock<ITenantObjectsManager> tenantObjectsManagerMock = null)
        {

            tokensManagerMock ??= TokensManagerTestsHelpers.GetTokensManagerMock();

            memoryCacheFacedeMock ??= MemoryCacheFacadeTestHelpers.GetMemoryCacheFacedeMock();

            tenantObjectsManagerMock ??= TenantObjectsManagerTestHelpers.GetTenantObjectsManagerMock();

            var authFilter = new AuthenticationFilter(tokensManagerMock.Object, memoryCacheFacedeMock.Object, tenantObjectsManagerMock.Object);

            return authFilter;
        }

        public static ActionExecutingContext GetActionExecutingContext()
        {
            string token = Guid.NewGuid().ToString();

            var actionArguments = new Dictionary<string, object>();

            var actionContext = new ActionContext();

            actionContext.HttpContext = new DefaultHttpContext();

            actionContext.RouteData = new RouteData();

            actionContext.ActionDescriptor = new ActionDescriptor();

            actionContext.HttpContext.Request.Headers[UrlAndContextPropertiesNames.AUTHENTICATION_HEADER_NAME] = token;

            var filters = new List<IFilterMetadata>();

            var context = new ActionExecutingContext(actionContext, filters, actionArguments, null);

            return context;
        }
    }
}
