using Mnemox.Account.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Timescale.DM.Dal;
using Mnemox.Timescale.Models;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Account
{
    public class UserRolesManagerTs: IUserRolesManager
    {
        private readonly ILogsManager _logsManager;
        private readonly IDbFactory _dbFactory;
        private readonly IDataManagersHelpersTs _dataManagersHelpers;

        private const string ADD_USER_ROLE_FNC_NAME = "tenants.users_roles_add";

        public UserRolesManagerTs(
            ILogsManager logsManager,
            IDbFactory dbFactory,
            IDataManagersHelpersTs dataManagersHelpers)
        {
            _logsManager = logsManager;
            _dbFactory = dbFactory;
            _dataManagersHelpers = dataManagersHelpers;

        }

        public async Task AddUserRole(long userId, short roleId)
        {
            IDbBase dbBase = null;

            try
            {
                dbBase = _dbFactory.GetDbBase();

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = userId,
                        ParameterName = _dataManagersHelpers.CreateParameterName(ConstantsTs.USER_ID),
                        NpgsqlDbType = NpgsqlDbType.Bigint
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = roleId,
                        ParameterName = _dataManagersHelpers.CreateParameterName(ConstantsTs.ROLE_ID),
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                await dbBase.ConnectAsync();

                await dbBase.ExecuteNonQueryFunctionAsync(ADD_USER_ROLE_FNC_NAME, parameters);

            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
            finally
            {
                if (dbBase != null)
                {
                    await dbBase.DisconnectAsync();
                }
            }
        }
    }
}
