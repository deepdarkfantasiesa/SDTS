using Domain.Abstraction;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Auth.API.Filters
{
    public class StronglyTypedIdSchemaFilter<T> : ISchemaFilter where T : IEntityTypeId<Guid>
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(T))
            {
                schema.Type = "string"; // 将类型设置为 string
                schema.Format = "uuid"; // 可选：指定格式为 UUID
            }
        }
    }
}
