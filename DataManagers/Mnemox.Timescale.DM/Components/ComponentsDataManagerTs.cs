using Microsoft.AspNetCore.Http;
using Mnemox.Components.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Components
{
    public class ComponentsDataManagerTs : IComponentsDataManager
    {
        private const string ADD_COMPONENT_FUNCTION_NAME = "components.components_add";

        private const string COMPONENT_NAME_PARAMETER_NAME = "p_component_name";

        private const string COMPONENT_DESCRIPTION_PARAMETER_NAME = "p_component_description";

        private const string COMPONENT_TYPE_PARAMETER_NAME = "p_component_type";

        private const string COMPONENT_NAME_IS_MANDATORY = "Component name is mandatory";

        private readonly ILogsManager _logsManager;

        private readonly IDbFactory _dbFactory;

        public ComponentsDataManagerTs(ILogsManager logsManager, IDbFactory dbFactory)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;
        }

        public async Task<long> AddComponent(ComponentBaseModel component)
        {
            IDbBase dbBase = null;

            try
            {
                if (string.IsNullOrWhiteSpace(component.Name))
                {
                    throw new OutputException(
                        new Exception(COMPONENT_NAME_IS_MANDATORY),
                        StatusCodes.Status400BadRequest,
                        MnemoxStatusCodes.INVALID_MODEL);
                }

                dbBase = _dbFactory.GetDbBase();

                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = component.Name,
                        ParameterName = COMPONENT_NAME_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = component.Description,
                        ParameterName = COMPONENT_DESCRIPTION_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Varchar
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = component.Type,
                        ParameterName = COMPONENT_TYPE_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                await dbBase.ConnectAsync();

                var componentId = (long)await dbBase.ExecuteScalarAsync(ADD_COMPONENT_FUNCTION_NAME, parameters);

                return componentId;
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
