using Microsoft.AspNetCore.Http;
using Mnemox.Account.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Account
{
    public class UsersDataManagerTsHelpers : IUsersDataManagerTsHelpers
    {
        private readonly ILogsManager _logsManager;
        private readonly IDbFactory _dbFactory;
        private readonly IDataManagersHelpers _dataManagersHelpers;

        private const string ALL_FIELDS_ARE_MANDATORY = "All fields are mandatory";
        private const string USERNAME_PARAMETER_NAME = "p_username";
        private const string PASSWORD_PARAMETER_NAME = "p_password";
        private const string SIGN_IN_USER_FUNCTION_NAME = "tenants.users_authenticate";
        private const string USER_ID_READER_FIELD_NAME = "o_user_id";
        private const string FIRST_NAME_READER_FIELD_NAME = "o_first_name";
        private const string LAST_NAME_READER_FIELD_NAME = "o_last_name";
        private const string USER_ID_PARAMETER_NAME = "p_user_id";

        public UsersDataManagerTsHelpers(ILogsManager logsManager, IDbFactory dbFactory, IDataManagersHelpers dataManagersHelpers)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;

            _dataManagersHelpers = dataManagersHelpers;
        }

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {
            IDbBase dbBase = null;

            User user = null;

            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    throw new OutputException(new Exception(ALL_FIELDS_ARE_MANDATORY))
                    {
                        HttpStatusCode = StatusCodes.Status400BadRequest,

                        MnemoxStatusCode = MnemoxStatusCodes.INVALID_MODEL
                    };
                }

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = username,
                        ParameterName = USERNAME_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = password,
                        ParameterName = PASSWORD_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    }
                };

                dbBase = _dbFactory.GetDbBase();

                await dbBase.ConnectAsync();

                using (var userReader = await dbBase.ExecuteReaderAsync(SIGN_IN_USER_FUNCTION_NAME, parameters))
                {
                    if (!userReader.HasRows)
                    {
                        return null;
                    }

                    userReader.Read();

                    user = new User
                    {
                        UserId = Convert.ToInt64(userReader[USER_ID_READER_FIELD_NAME]),

                        FirstName = _dataManagersHelpers.GetString(userReader, FIRST_NAME_READER_FIELD_NAME),

                        LastName = _dataManagersHelpers.GetString(userReader, LAST_NAME_READER_FIELD_NAME)
                    };
                }

                return user;
            }
            catch (OutputException)
            {
                throw;
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

        public async Task SetSignedInUser(User user, string signInToken)
        {
            try
            {
                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = user.UserId,
                        ParameterName = USER_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        //NpgsqlValue = password,
                        //ParameterName = PASSWORD_PARAMETER_NAME,
                        //NpgsqlDbType = NpgsqlDbType.Varchar
                    }
                };
            }
            catch(Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
        }
    }
}
