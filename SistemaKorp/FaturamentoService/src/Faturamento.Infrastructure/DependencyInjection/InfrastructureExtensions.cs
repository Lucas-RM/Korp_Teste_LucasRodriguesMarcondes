using System.Reflection;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Interfaces;
using Faturamento.Infrastructure.ExternalServices;
using Faturamento.Infrastructure.Persistence.Context;
using Faturamento.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Faturamento.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext PostgreSQL
        var connectionString = configuration.GetConnectionString("FaturamentoConnection");
        services.AddDbContext<FaturamentoDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null
                )
            )
        );

        // Repositórios
        services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FaturamentoDbContext>());

        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // HttpClient com Polly para Serviço de Estoque
        var estoqueBaseUrl = configuration["EstoqueService:BaseUrl"];
        services.AddHttpClient<IEstoqueService, EstoqueHttpService>(client =>
        {
            client.BaseAddress = new Uri(estoqueBaseUrl!);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler((sp, _) =>
        {
            var logger = sp.GetRequiredService<ILogger<EstoqueHttpService>>();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (outcome, timespan, retryAttempt, _) =>
                        logger.LogWarning(
                            "[Polly] Retry {Attempt}/3 | Delay {Delay}s | Erro: {Error}",
                            retryAttempt, timespan.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()));
        });

        return services;
    }
}

