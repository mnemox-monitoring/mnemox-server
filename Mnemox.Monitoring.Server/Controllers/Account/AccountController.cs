using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnemox.Account.Models;
using Mnemox.Account.Models.Enums;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers.Account
{
    [Route("account")]
    [ApiController]
    public class AccountController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        private readonly IUsersDataManager _usersDataManager;
        private readonly IUserRolesManager _userRolesManager;
        private const string OWNER_EXISTS_ALREADY = "Owner exists already";

        public AccountController(ILogsManager logsManager, IUsersDataManager usersDataManager, IUserRolesManager userRolesManager)
        {
            _logsManager = logsManager;

            _usersDataManager = usersDataManager;

            _userRolesManager = userRolesManager;
        }

        /// <summary>
        /// Sign up owner user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("owner")]
        public async Task<IActionResult> OwnerSignUp([FromBody]UserSignUp userSignUp)
        {
            try
            {
                var systemOwner = await _usersDataManager.GetUsersByRole(RolesEnum.SystemOwner);

                if(systemOwner != null)
                {
                    throw new OutputException(
                        new Exception(OWNER_EXISTS_ALREADY),
                        StatusCodes.Status401Unauthorized,
                        MnemoxStatusCodes.OWNER_EXISTS_ALREADY);
                }

                var userId = await _usersDataManager.CreateUser(userSignUp);

                await _userRolesManager.AddUserRole(userId, (short)RolesEnum.SystemOwner);

                return Ok(new { userId });
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
