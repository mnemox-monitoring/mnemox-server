using System.Threading.Tasks;

namespace Mnemox.Components.Models
{
    public interface IComponentsDataManager
    {
        Task AddComponents(ComponentBaseModel component);
    }
}
