using Microsoft.AspNetCore.Http;
using Mnemox.Account.Models;
using Mnemox.Account.Models.Enums;
using Mnemox.Logs.Models;
using Mnemox.Security.Utils;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using Mnemox.Timescale.Models;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Account
{
    public class UsersDataManagerTs : IUsersDataManager
    {
        private readonly ILogsManager _logsManager;
        private readonly IUsersDataManagerTsHelpers _usersDataManagerTsHelpers;
        private readonly ISecretsManager _secretsManager;
        private readonly IDbFactory _dbFactory;
        private readonly IDataManagersHelpersTs _dataManagersHelpers;

        private const short TOKEN_VALID_MINUTES = 60;
        private const string INVALID_USERNAME_OR_PASSWORD = "Invalid username or password";

        private const string GET_USERS_BY_ROLE_FNC_NAME = "tenants.users_get_by_role";

        public UsersDataManagerTs(
            ILogsManager logsManager, 
            IUsersDataManagerTsHelpers usersDataManagerTsHelpers,
            ISecretsManager secretsManager,
            IDbFactory dbFactory,
            IDataManagersHelpersTs dataManagersHelpers)
        {
            _logsManager = logsManager;

            _usersDataManagerTsHelpers = usersDataManagerTsHelpers;

            _secretsManager = secretsManager;

            _dbFactory = dbFactory;

            _dataManagersHelpers = dataManagersHelpers;
        }

        public async Task<AuthResponse> SignIn(AuthRequest authRequest)
        {
            try
            {
                var user = await _usersDataManagerTsHelpers.GetUserByUsernameAndPassword(authRequest.Username, authRequest.Password);

                if(user == null)
                {
                    throw new OutputException(
                        new Exception(INVALID_USERNAME_OR_PASSWORD),
                        StatusCodes.Status401Unauthorized,
                        MnemoxStatusCodes.INVALID_USERNAME_OR_PASSWORD);
                }

                var token = _secretsManager.GenerateToken();

                var tokenValidUntilUtc = DateTime.UtcNow.AddMinutes(TOKEN_VALID_MINUTES);

                var tokenId = await _usersDataManagerTsHelpers.SetSignedInUserIntoStorage(user, token, tokenValidUntilUtc);

                return new AuthResponse
                {
                    Token = token
                };
            }
            catch (OutputException)
            {
                throw;
            }
            catch (HandledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new HandledException(ex);
            }
        }

        public async Task<List<User>> GetUsersByRole(RolesEnum rolesEnum)
        {
            IDbBase dbBase = null;

            List<User> users = null;

            try
            {
                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = (short)rolesEnum,
                        ParameterName = _dataManagersHelpers.CreateParameterName(ConstantsTs.ROLE_ID),
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                dbBase = _dbFactory.GetDbBase();

                await dbBase.ConnectAsync();

                using (var reader = await dbBase.ExecuteReaderFunctionAsync(GET_USERS_BY_ROLE_FNC_NAME, parameters))
                {
                    if (reader.HasRows)
                    {
                        users = new List<User>();

                        while (reader.Read())
                        {
                            var email = reader[_dataManagersHelpers.CreateSelectPropertyName(ConstantsTs.EMAIL)];
                            var firstName = reader[_dataManagersHelpers.CreateSelectPropertyName(ConstantsTs.FIRST_NAME)];
                            var lastName = reader[_dataManagersHelpers.CreateSelectPropertyName(ConstantsTs.LAST_NAME)];

                            users.Add( 
                                new User
                                {
                                    UserId = Convert.ToInt64(_dataManagersHelpers.CreateSelectPropertyName(ConstantsTs.USER_ID)),
                                    Email = email is DBNull ? null : email.ToString(),
                                    FirstName = firstName is DBNull ? null : firstName.ToString(),
                                    LastName = lastName is DBNull ? null : lastName.ToString()
                                }
                            );
                        }
                    }
                }

                return users;
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
