using System.Threading.Tasks;

namespace Mnemox.Shared.Models.Settings
{
    public interface ISettingsManager
    {
        IFullSettings FullSettings { get; }

        Task ReloadSettingsAsync();
    }
}