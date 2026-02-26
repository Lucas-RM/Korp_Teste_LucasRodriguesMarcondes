using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Faturamento.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Faturamento API",
                Version = "v1",
                Description = "API do Serviço de Faturamento - Sistema de Emissão de Notas Fiscais",
                Contact = new OpenApiContact
                {
                    Name = "Equipe de Desenvolvimento",
                    Email = "dev@empresa.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Faturamento API v1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}

