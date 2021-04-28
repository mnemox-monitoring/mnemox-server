using Microsoft.OpenApi.Models;
using Mnemox.Shared.Models.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Mnemox.Api.Security.Utils
{
    public class SwaggerAuthorizationHeaders : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = UrlAndContextPropertiesNames.AUTHENTICATION_HEADER_NAME,
                In = ParameterLocation.Header,
                Required = false
            });
        }
    }
}
