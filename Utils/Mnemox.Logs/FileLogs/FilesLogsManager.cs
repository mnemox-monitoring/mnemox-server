using Mnemox.Logs.Models;
using Mnemox.Logs.Models.FilesLogs;
using System.Threading.Tasks;

namespace Mnemox.Logs.Utils.FileLogs
{
    public class FilesLogsManager : ILogsManager
    {
        private readonly FilesLogsConfiguration _configuration;

        public FilesLogsManager(FilesLogsConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        public async Task ErrorAsync(ErrorLogStructure errorLogStructure)
        {
            
        }
    }
}
