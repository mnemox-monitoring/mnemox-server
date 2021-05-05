using Mnemox.Account.Models;
using Moq;

namespace Shared
{
    public class TokensManagerTestsHelpers
    {
        public static Mock<ITokensManager> GetTokensManagerMock()
        {
            var mock = new Mock<ITokensManager>();

            return mock;
        }
    }
}
