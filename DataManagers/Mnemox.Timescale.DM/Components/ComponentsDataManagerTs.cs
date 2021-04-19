using Mnemox.Components.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Timescale.DM.Dal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Components
{
    public class ComponentsDataManagerTs : IComponentsDataManager
    {
        private readonly ILogsManager _logsManager;

        private readonly IDbFactory _dbFactory;

        public ComponentsDataManagerTs(ILogsManager logsManager, IDbFactory dbFactory)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;
        }

        public async Task AddComponents(ComponentBaseModel component)
        {
            IDbBase dbBase = null;

            try
            {
                dbBase = _dbFactory.GetDbBase();

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        //NpgsqlValue = heartBeatRequest.InstanceId,
                        //ParameterName = INSTANCE_ID_PARAMETER_NAME,
                        //NpgsqlDbType = NpgsqlDbType.Bigint
                    }
                };

                await dbBase.ConnectAsync();

                //await dbBase.ExecuteNonQueryAsync(ADD_HEART_BEAT_FUNCTION_NAME, parameters);
            }
            catch (Exception ex)
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
