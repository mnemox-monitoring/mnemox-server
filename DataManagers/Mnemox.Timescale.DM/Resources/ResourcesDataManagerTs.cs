using Microsoft.AspNetCore.Http;
using Mnemox.Logs.Models;
using Mnemox.Resources.Models;
using Mnemox.Shared.Models;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Resources
{
    public class ResourcesDataManagerTs : IResourceDataManager
    {
        private const string ADD_RESOURCE_FUNCTION_NAME = "resources.resources_add";

        private const string RESOURCE_NAME_PARAMETER_NAME = "p_resource_name";

        private const string RESOURCE_DESCRIPTION_PARAMETER_NAME = "p_resource_description";

        private const string RESOURCE_TYPE_PARAMETER_NAME = "p_resource_type";

        private const string RESOURCE_NAME_IS_MANDATORY = "Resource name is mandatory";

        private readonly ILogsManager _logsManager;

        private readonly IDbFactory _dbFactory;

        public ResourcesDataManagerTs(ILogsManager logsManager, IDbFactory dbFactory)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;
        }

        public async Task<long> AddResource(ResourceBaseModel resource)
        {
            IDbBase dbBase = null;

            try
            {
                if (string.IsNullOrWhiteSpace(resource.Name))
                {
                    throw new OutputException(
                        new Exception(RESOURCE_NAME_IS_MANDATORY),
                        StatusCodes.Status400BadRequest,
                        MnemoxStatusCodes.INVALID_MODEL);
                }

                dbBase = _dbFactory.GetDbBase();

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = resource.Name,
                        ParameterName = RESOURCE_NAME_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = resource.Description,
                        ParameterName = RESOURCE_DESCRIPTION_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = resource.Type,
                        ParameterName = RESOURCE_TYPE_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                await dbBase.ConnectAsync();

                var resourceId = (long)await dbBase.ExecuteScalarAsync(ADD_RESOURCE_FUNCTION_NAME, parameters);

                return resourceId;
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
