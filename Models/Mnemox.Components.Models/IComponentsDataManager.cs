using System.Threading.Tasks;

namespace Mnemox.Components.Models
{
    public interface IComponentsDataManager
    {
        Task<long> AddComponent(ComponentBaseModel component);
    }
}
