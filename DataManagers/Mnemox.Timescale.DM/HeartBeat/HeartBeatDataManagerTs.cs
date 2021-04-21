using Mnemox.Logs.Models;
using Mnemox.Monitoring.Models;
using Mnemox.Shared.Models;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.HeartBeat
{
    public class HeartBeatDataManagerTs : IHeartBeatDataManager
    {
        private readonly IDbFactory _dbFactory;

        private readonly ILogsManager _logsManager;

        private const string INSTANCE_ID_PARAMETER_NAME = "p_instance_id";

        private const string ADD_HEART_BEAT_FUNCTION_NAME = "monitoring.heart_beats_add";

        public HeartBeatDataManagerTs(IDbFactory dbFactory, ILogsManager logsManager)
        {
            _dbFactory = dbFactory;

            _logsManager = logsManager;
        }

        public async Task StoreHeartBeat(HeartBeatRequest heartBeatRequest)
        {
            IDbBase dbBase = null;

            try
            {
                dbBase = _dbFactory.GetDbBase();

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = heartBeatRequest.InstanceId,
                        ParameterName = INSTANCE_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Bigint
                    }
                };

                await dbBase.ConnectAsync();

                await dbBase.ExecuteNonQueryAsync(ADD_HEART_BEAT_FUNCTION_NAME, parameters);
            }
            catch(Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
            finally
            {
                await dbBase?.DisconnectAsync();
            }
        }
    }
}
