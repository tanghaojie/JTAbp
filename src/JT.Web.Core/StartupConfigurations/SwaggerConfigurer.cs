using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Security.Policy;

namespace JT.Web.Core.StartupConfigurations
{
    public class SwaggerConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.Parse(configuration["Swagger:IsEnabled"]))
            {
                var version = configuration["Swagger:Version"];
                var title = configuration["Swagger:Title"];
                var description = configuration["Swagger:Description"];
                var termsOfServiceUrl = configuration["Swagger:TermsOfServiceUrl"];

                var contactName = configuration["Swagger:ContactName"];
                var contactEmail = configuration["Swagger:ContactEmail"];
                var contactUrl = configuration["Swagger:ContactUrl"];
                OpenApiContact contact = null;
                if (
                    !string.IsNullOrWhiteSpace(contactName) ||
                    !string.IsNullOrWhiteSpace(contactEmail) ||
                    !string.IsNullOrWhiteSpace(contactUrl))
                {
                    contact = new OpenApiContact
                    {
                        Name = contactName,
                        Email = contactEmail,
                        Url = string.IsNullOrWhiteSpace(contactUrl) ? null : new Uri(contactUrl)
                    };
                }

                var licenseName = configuration["Swagger:LicenseName"];
                var licenseUrl = configuration["Swagger:LicenseUrl"];
                OpenApiLicense license = null;
                if (
                    !string.IsNullOrWhiteSpace(licenseName) ||
                    !string.IsNullOrWhiteSpace(licenseUrl))
                {
                    license = new OpenApiLicense
                    {
                        Name = licenseName,
                        Url = string.IsNullOrWhiteSpace(licenseUrl) ? null : new Uri(licenseUrl),
                    };
                }

                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = title,
                        Version = version,
                        Description = description,
                        TermsOfService = string.IsNullOrWhiteSpace(termsOfServiceUrl) ? null : new Uri(termsOfServiceUrl),
                        Contact = contact,
                        License = license
                    });
                    options.DocInclusionPredicate((docName, description) => true);
                    //options.CustomSchemaIds(x => x.FullName);
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                    });
                });
            }
        }

        public static void Use(IApplicationBuilder app, IConfiguration configuration)
        {
            if (bool.Parse(configuration["Swagger:IsEnabled"]))
            {
                var version = configuration["Swagger:Version"];
                var title = configuration["Swagger:Title"];

                app.UseSwagger(options => { options.RouteTemplate = "swagger/{documentName}/swagger.json"; });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{title} {version}");
                    options.DisplayRequestDuration();
                });
            }
        }
    }
}
