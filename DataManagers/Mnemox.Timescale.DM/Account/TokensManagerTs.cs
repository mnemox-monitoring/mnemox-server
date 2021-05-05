using Mnemox.Account.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Utils;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Account
{
    public class TokensManagerTs : ITokensManager
    {
        private const string GET_TOKEN_DETAILS_FUNCTION_NAME = "tenants.tokens_get_details";
        
        private const string TOKEN_PARAMTER_NAME = "p_token";

        private const string OUTPUT_TOKEN_PARAMTER_NAME = "o_token";
        private const string OUTPUT_TOKEN_ID_PARAMTER_NAME = "o_token_id";
        private const string OUTPUT_TOKEN_OWNER_ID_PARAMTER_NAME = "o_owner_id";
        private const string OUTPUT_TOKEN_OWNER_TYPE_ID_PARAMTER_NAME = "o_owner_type_id";
        private const string OUTPUT_TOKEN_VALID_UNTIL_PARAMTER_NAME = "o_valid_until_utc";
        private readonly ILogsManager _logsManager;
        private readonly IDbFactory _dbFactory;
        private readonly IDataManagersHelpersTs _dataManagersHelpers;
        
        public TokensManagerTs(
            ILogsManager logsManager,
            IDbFactory dbFactory,
            IDataManagersHelpersTs dataManagersHelpers)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;

            _dataManagersHelpers = dataManagersHelpers;

        }

        public async Task<Tokens> GetTokenDetailsFromDataStorgeAsync(string token)
        {
            IDbBase dbBase = null;

            Tokens tokenResponse = null;

            try
            {
                dbBase = _dbFactory.GetDbBase();

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = token,
                        ParameterName = TOKEN_PARAMTER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    }
                };

                await dbBase.ConnectAsync();

                using (var reader = await dbBase.ExecuteReaderAsync(GET_TOKEN_DETAILS_FUNCTION_NAME, parameters))
                {
                    if (reader.HasRows && reader.Read())
                    {
                        tokenResponse = new Tokens
                        {
                            TokenId = _dataManagersHelpers.GetLong(reader, OUTPUT_TOKEN_ID_PARAMTER_NAME).Value,

                            Token = _dataManagersHelpers.GetString(reader, OUTPUT_TOKEN_PARAMTER_NAME),

                            OwnerId = _dataManagersHelpers.GetLong(reader, OUTPUT_TOKEN_OWNER_ID_PARAMTER_NAME).Value,

                            MnemoxAccessObjectsType = (MnemoxAccessObjectsTypesEnum)_dataManagersHelpers.GetInt(reader, OUTPUT_TOKEN_OWNER_TYPE_ID_PARAMTER_NAME).Value,

                            ValidUntilDateTimeUtc = _dataManagersHelpers.GetDateTime(reader, OUTPUT_TOKEN_VALID_UNTIL_PARAMTER_NAME).Value
                        };
                    }
                }

                return tokenResponse;
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

        public double GetTokenTtlMinutes(DateTime tokenValidUntilDateTimeUtc)
        {
            return (tokenValidUntilDateTimeUtc.ToLocalTime() - DateTime.Now).TotalMinutes;
        }
    }
}
