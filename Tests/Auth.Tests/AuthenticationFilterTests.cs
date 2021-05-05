using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mnemox.Account.Models;
using Mnemox.Shared.Models.Enums;
using Moq;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Auth.Tests
{
    public class AuthenticationFilterTests
    {
        [Fact]
        public async Task RequestOwner_RetrievedFromDataStorage_IfItNotInCache()
        {
            var tokensManager = TokensManagerTestsHelpers.GetTokensManagerMock();

            var target = AuthenticationFilterTestsHelpers.GetTarget(tokensManagerMock: tokensManager);

            await target.OnActionExecutionAsync(AuthenticationFilterTestsHelpers.GetActionExecutingContext(), null);

            tokensManager.Verify(x => x.GetTokenDetailsFromDataStorgeAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ReturnsUnauthorizedRequest_IfRequestOwnerNotInCache_And_TokenDataNotInDataStorage()
        {
            var tokensManager = TokensManagerTestsHelpers.GetTokensManagerMock();

            var target = AuthenticationFilterTestsHelpers.GetTarget(tokensManagerMock: tokensManager);

            var context = AuthenticationFilterTestsHelpers.GetActionExecutingContext();

            await target.OnActionExecutionAsync(context, null);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public async Task ReturnsUnauthorizedRequest_IfRequestOwner_HasNoTenantsRelation()
        {
            var tokensManager = TokensManagerTestsHelpers.GetTokensManagerMock();

            var tokenDetails = new Tokens();

            tokensManager.Setup(x => x.GetTokenDetailsFromDataStorgeAsync(It.IsAny<string>())).ReturnsAsync(tokenDetails);

            var tenantsManager = TenantObjectsManagerTestHelpers.GetTenantObjectsManagerMock();

            var target = AuthenticationFilterTestsHelpers.GetTarget(
                tokensManagerMock: tokensManager, 
                tenantObjectsManagerMock: tenantsManager);

            var context = AuthenticationFilterTestsHelpers.GetActionExecutingContext();

            await target.OnActionExecutionAsync(context, null);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public async Task TokenSetIntoCache_IfRequestOwner_RetrievedFromDataStorage()
        {
            var tokensManager = TokensManagerTestsHelpers.GetTokensManagerMock();

            var tokenDetails = new Tokens
            {
                OwnerId = 1,

                MnemoxAccessObjectsType = MnemoxAccessObjectsTypesEnum.USER,
            };

            tokensManager.Setup(x => x.GetTokenDetailsFromDataStorgeAsync(It.IsAny<string>())).ReturnsAsync(tokenDetails);

            double tokenTtlMinutes = 60;

            tokensManager.Setup(x => x.GetTokenTtlMinutes(It.IsAny<DateTime>())).Returns(tokenTtlMinutes);
            
            var tenantsManager = TenantObjectsManagerTestHelpers.GetTenantObjectsManagerMock();

            var tenants = new List<Tenant>();

            tenantsManager.Setup(x => x.GetObjectTenantsAsync(
                It.IsAny<long>(), 
                It.IsAny<MnemoxAccessObjectsTypesEnum>())).
                ReturnsAsync(tenants);

            var cacheFacade = MemoryCacheFacadeTestHelpers.GetMemoryCacheFacedeMock();

            var target = AuthenticationFilterTestsHelpers.GetTarget(
                tokensManagerMock: tokensManager,
                tenantObjectsManagerMock: tenantsManager,
                memoryCacheFacedeMock: cacheFacade);

            var context = AuthenticationFilterTestsHelpers.GetActionExecutingContext();

            var metadata = new List<IFilterMetadata>();

            ActionExecutionDelegate next = () => {
                var ctx = new ActionExecutedContext(context, metadata, null);
                return Task.FromResult(ctx);
            };

            await target.OnActionExecutionAsync(context, next);

            cacheFacade.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<RequestOwner>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
