using Microsoft.AspNetCore.Mvc;
using Mnemox.Account.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers.Account
{
    [Route("auth")]
    [ApiController]
    public class AuthController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        private readonly IUsersDataManager _usersDataManager;

        public AuthController(ILogsManager logsManager, IUsersDataManager usersDataManager)
        {
            _logsManager = logsManager;

            _usersDataManager = usersDataManager;
        }

        /// <summary>
        /// Sign-in Mnemox Monitoring Server users
        /// </summary>
        /// <returns></returns>
        [Route("sign-in")]
        [HttpPost]
        public async Task<ActionResult<AuthResponse>> SignIn([FromBody]AuthRequest authRequest)
        {
            try
            {
                var response = await _usersDataManager.SignIn(authRequest);

                return Ok(response);
            }
            catch (OutputException ex)
            {
                return CreateErrorResultFromOutputException(ex);
            }
            catch (HandledException)
            {
                return InternalServerErrorResult();
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                return InternalServerErrorResult();
            }
        }
    }
}
