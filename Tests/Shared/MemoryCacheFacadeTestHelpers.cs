using Mnemox.Shared.Utils;
using Moq;

namespace Shared
{
    public class MemoryCacheFacadeTestHelpers
    {
        public static Mock<IMemoryCacheFacade> GetMemoryCacheFacedeMock()
        {
            var mock = new Mock<IMemoryCacheFacade>();

            return mock;
        }
    }
}
