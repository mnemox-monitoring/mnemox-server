using System.Threading.Tasks;

namespace Mnemox.Resources.Models
{
    public interface IResourceDataManager
    {
        Task<long> AddResource(ResourceBaseModel resource);
    }
}
