using System;
using System.Threading.Tasks;

namespace Mnemox.Logs.Models
{
    public interface ILogsManager
    {
        Task ErrorAsync(ErrorLogStructure errorLogStructure);
    }
}
