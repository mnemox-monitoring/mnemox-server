using Microsoft.AspNetCore.Http;
using Mnemox.DataStorage.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Infrastructure
{
    public class ServersManagerTs: IServersManager
    {
        private readonly ILogsManager _logsManager;

        private readonly IDbFactory _dbFactory;

        private const string STATE_ID_PARAMETER_NAME = "p_state_id";

        private const string SERVER_ID_PARAMETER_NAME = "p_server_id";

        private const string SERVER_NAME_PARAMTER_NAME = "p_server_name";

        private readonly string GET_SERVERS_LIST_FNC_NAME = "server.servers_get_by_state_or_all";

        private readonly string GET_SERVER_DETAILS_BY_ID_FNC_NAME = "server.servers_get_details_by_id";

        private readonly string ADD_SERVER_FNC_NAME = "server.servers_add";

        private const string SET_INITIALIZATION_STATE_FNC_NAME = "server.initialization_states_set";

        private readonly string GET_LAST_INITIALIZATION_STATE = "server.initialization_states_get_last";

        public ServersManagerTs(ILogsManager logsManager, IDbFactory dbFactory)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;
        }

        public async Task<List<ServerModel>> GetServersListByStateOrAllAsync(StatesEnums? state = null)
        {
            IDbBase db = null;

            try 
            {
                List<ServerModel> servers = null;

                var parameters = new List<TimescaleParameter> {
                    new TimescaleParameter
                    {
                        NpgsqlValue = state != null ? (short)state : DBNull.Value,
                        ParameterName = STATE_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                db = _dbFactory.GetDbBase();

                await db.ConnectAsync();

                using (var reader = await db.ExecuteReaderFunctionAsync(GET_SERVERS_LIST_FNC_NAME, parameters))
                {
                    if (reader.HasRows)
                    {
                        servers = new List<ServerModel>();

                        while (reader.Read())
                        {
                            servers.Add(new ServerModel
                            {
                                ServerId = Convert.ToInt64(reader["o_server_id"]),
                                ServerName = reader["o_server_name"].ToString(),
                                ServerState = (StatesEnums)Convert.ToInt16(reader["o_state_id"])
                            });
                        }
                    }
                }

                return servers;
            }
            catch(Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
            finally
            {
                await db?.DisconnectAsync();
            }
        }

        public async Task<ServerModel> GetServerDetailsById(long serverId)
        {
            IDbBase db = null;

            try
            {
                ServerModel server = null;

                var parameters = new List<TimescaleParameter> {
                    new TimescaleParameter
                    {
                        NpgsqlValue = serverId,
                        ParameterName = SERVER_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Bigint
                    }
                };

                db = _dbFactory.GetDbBase();

                await db.ConnectAsync();

                using (var reader = await db.ExecuteReaderFunctionAsync(GET_SERVER_DETAILS_BY_ID_FNC_NAME, parameters))
                {
                    if (reader.HasRows && reader.Read())
                    {

                        server = new ServerModel
                        {
                            ServerId = Convert.ToInt64(reader["o_server_id"]),
                            ServerName = reader["o_server_name"].ToString(),
                            ServerState = (StatesEnums)Convert.ToInt16(reader["o_state_id"])
                        };
                        
                    }
                }

                return server;
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
            finally
            {
                await db?.DisconnectAsync();
            }
        }

        public async Task<int> Add(ServerModel serverModel)
        {
            IDbBase db = null;

            try
            {
                var parameters = new List<TimescaleParameter> {
                    new TimescaleParameter
                    {
                        NpgsqlValue = serverModel.ServerName,
                        ParameterName = SERVER_NAME_PARAMTER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = (short)serverModel.ServerState,
                        ParameterName = STATE_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                db = _dbFactory.GetDbBase();

                await db.ConnectAsync();

                var serverId = (int)await db.ExecuteScalarAsync(ADD_SERVER_FNC_NAME, parameters);

                return serverId;
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
            finally
            {
                await db?.DisconnectAsync();
            }
        }

        public async Task ServerInitizalizationStateSet(InitializationStatesEnums databaseState)
        {
            IDbBase dbBase = null;

            try
            {
                var parameters = new List<TimescaleParameter> {
                    new TimescaleParameter
                    {
                        NpgsqlValue = (short)databaseState,
                        ParameterName = STATE_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                dbBase = _dbFactory.GetDbBase();

                await dbBase.ConnectAsync();

                await dbBase.ExecuteNonQueryFunctionAsync(SET_INITIALIZATION_STATE_FNC_NAME, parameters);

            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    ex,
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_SET_STATE_PARAMETER);
            }
            finally
            {
                await dbBase?.DisconnectAsync();
            }
        }

        public async Task<StatesEnums?> GetLastInitializationState()
        {
            IDbBase db = null;

            StatesEnums? statesEnums = null;

            try
            {
                db = _dbFactory.GetDbBase();

                await db.ConnectAsync();

                using (var reader = await db.ExecuteReaderFunctionAsync(GET_LAST_INITIALIZATION_STATE))
                {
                    if (reader.HasRows && reader.Read())
                    {
                        statesEnums = (StatesEnums)Convert.ToInt16(reader["o_state_id"]);
                    }
                }

                return statesEnums;
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
            finally
            {
                await db?.DisconnectAsync();
            }
        }
    }
}
