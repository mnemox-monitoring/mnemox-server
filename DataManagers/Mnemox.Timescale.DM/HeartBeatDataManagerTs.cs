using Mnemox.HeartBeat.Models;
using Mnemox.Logs.Models;
using Mnemox.Timescale.DM.Dal;
using System;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM
{
    public class HeartBeatDataManagerTs : IHeartBeatDataManager
    {
        private readonly IDbFactory _dbFactory;

        private readonly ILogsManager _logsManager;

        public HeartBeatDataManagerTs(IDbFactory dbFactory, ILogsManager logsManager)
        {
            _dbFactory = dbFactory;

            _logsManager = logsManager;
        }

        public async Task StoreHeartBeat(HeartBeatRequest heartBeatRequest)
        {
            try
            {

            }
            catch(Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex));

                throw;
            }
        }
    }
}
