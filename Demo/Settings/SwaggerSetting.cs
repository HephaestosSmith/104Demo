using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Demo.Settings
{
        /// <summary>
        /// Swagger 服務擴充方法
        /// </summary>
        public static class SwaggerSetting
        {
            /// <summary>
            /// This method to add Swagger services to service container.
            /// </summary>
            /// <param name="services">IServiceCollection</param>
            /// <returns>IServiceCollection instance</returns>
            public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
            {

                services.AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Demo",
                        Version = $"{DateTime.Now}",
                    });
                    // Authorization
                    c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization",
                    });
                    c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                     {
                        new OpenApiSecurityScheme
                          {
                            Reference = new OpenApiReference
                              {
                                                Type = ReferenceType.SecurityScheme,
                                                Id = "Bearer",
                              },
                          },
                        Array.Empty<string>()
                     },
                    });
                    c.OperationFilter<ProducesResponseTypeFilter>();
                    c.DocumentFilter<TagDescriptionsDocumentFilter>();

                });

                return services;
            }
        }

        public class ProducesResponseTypeFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Responses == null)
                {
                    operation.Responses = new OpenApiResponses();
                }

                TryAddResponse(operation, StatusCodes.Status400BadRequest, "The server could not understand the request.");
                TryAddResponse(operation, StatusCodes.Status401Unauthorized, "The request is unauthorized.");
                TryAddResponse(operation, StatusCodes.Status500InternalServerError, "Occurred internal server error.");
            }

            private void TryAddResponse(OpenApiOperation operation, int httpStatus, string description)
            {
                string httpStatusStr = httpStatus.ToString();
                if (!operation.Responses.ContainsKey(httpStatusStr))
                {
                    operation.Responses.Add(
                        httpStatusStr,
                        new OpenApiResponse { Description = description });
                }
            }
        }

        public class TagDescriptionsDocumentFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                var tagName = nameof(Demo);
                swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = tagName, Description = "category of systems" },
            };
            }
        }
    
}
