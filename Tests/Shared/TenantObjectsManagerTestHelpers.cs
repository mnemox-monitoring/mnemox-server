using Mnemox.Account.Models;
using Moq;

namespace Shared
{
    public class TenantObjectsManagerTestHelpers
    {
        public static Mock<ITenantObjectsManager> GetTenantObjectsManagerMock()
        {
            var mock = new Mock<ITenantObjectsManager>();

            return mock;
        }
    }
}
