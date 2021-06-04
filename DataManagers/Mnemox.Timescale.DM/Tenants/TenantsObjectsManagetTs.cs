using Mnemox.Account.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Tenants
{
    public class TenantsObjectsManagetTs : ITenantObjectsManager
    {
        private const string GET_OBJECT_TENANTS_FUNCTION_NAME = "tenants.tenants_objects_get_by_object";

        private const string OBJECT_ID_PARAMETER_NAME = "p_object_id";
        private const string OBJECT_TYPE_ID_PARAMETER_NAME = "p_object_type_id";

        private const string TENANT_ID_FIELD_NAME = "o_tenant_id";
        private const string TENANT_NAME_FIELD_NAME = "o_tenant_name";
        private const string TENANT_DESCRIPTION_FIELD_NAME = "o_tenant_description";
        private readonly ILogsManager _logsManager;
        private readonly IDbFactory _dbFactory;
        private readonly IDataManagersHelpersTs _dataManagersHelpers;

        public TenantsObjectsManagetTs(
            ILogsManager logsManager,
            IDbFactory dbFactory,
            IDataManagersHelpersTs dataManagersHelpers)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;

            _dataManagersHelpers = dataManagersHelpers;
        }

        public async Task<List<Tenant>> GetObjectTenantsAsync(long objectId, MnemoxAccessObjectsTypesEnum objectType)
        {
            IDbBase dbBase;

            List<Tenant> objectTenants = null;

            try
            {
                var parameters = new List<TimescaleParameter>
                {
                    new TimescaleParameter
                    {
                        NpgsqlValue = objectId,
                        ParameterName = OBJECT_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Bigint
                    },
                    new TimescaleParameter
                    {
                        NpgsqlValue = (short)objectType,
                        ParameterName = OBJECT_TYPE_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                dbBase = _dbFactory.GetDbBase();

                await dbBase.ConnectAsync();

                using (var reader = await dbBase.ExecuteReaderFunctionAsync(GET_OBJECT_TENANTS_FUNCTION_NAME, parameters))
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    objectTenants = new List<Tenant>();

                    while (reader.Read())
                    {
                        var tenant = new Tenant
                        {
                            TenantId = Convert.ToInt64(reader[TENANT_ID_FIELD_NAME]),

                            Name = _dataManagersHelpers.GetString(reader, TENANT_NAME_FIELD_NAME),

                            Description = _dataManagersHelpers.GetString(reader, TENANT_DESCRIPTION_FIELD_NAME)
                        };

                        objectTenants.Add(tenant);
                    }
                }

                return objectTenants;
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
