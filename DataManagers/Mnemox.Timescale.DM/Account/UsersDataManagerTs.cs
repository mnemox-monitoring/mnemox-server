using Microsoft.AspNetCore.Http;
using Mnemox.Account.Models;
using Mnemox.Logs.Models;
using Mnemox.Security.Utils;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
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
        private const string INVALID_USERNAME_OR_PASSWORD = "Invalid username or password";


        public UsersDataManagerTs(
            ILogsManager logsManager, 
            IUsersDataManagerTsHelpers usersDataManagerTsHelpers,
            ISecretsManager secretsManager)
        {
            _logsManager = logsManager;

            _usersDataManagerTsHelpers = usersDataManagerTsHelpers;

            _secretsManager = secretsManager;
        }

        public async Task<AuthResponse> SignIn(AuthRequest authRequest)
        {
            try
            {
                var user = await _usersDataManagerTsHelpers.GetUserByUsernameAndPassword(authRequest.Username, authRequest.Password);

                if(user == null)
                {
                    throw new OutputException(new Exception(INVALID_USERNAME_OR_PASSWORD)) { 
                        HttpStatusCode = StatusCodes.Status404NotFound,
                        MnemoxStatusCode = MnemoxStatusCodes.INVALID_USERNAME_OR_PASSWORD
                    };
                }

                var token = _secretsManager.GenerateToken();

                await _usersDataManagerTsHelpers.SetSignedInUser(user, token);

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

        
    }
}
