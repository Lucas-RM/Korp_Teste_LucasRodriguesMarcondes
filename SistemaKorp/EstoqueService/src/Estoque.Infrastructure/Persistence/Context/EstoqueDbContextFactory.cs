using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Estoque.Infrastructure.Persistence.Context;

public class EstoqueDbContextFactory : IDesignTimeDbContextFactory<EstoqueDbContext>
{
    public EstoqueDbContext CreateDbContext(string[] args)
    {
        // Obtém o diretório do assembly atual (Estoque.Infrastructure)
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        var basePath = Path.GetFullPath(
            Path.Combine(assemblyDirectory ?? string.Empty, "..", "..", "..", "..", "Estoque.API")
        );

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("EstoqueConnection")
            ?? Environment.GetEnvironmentVariable("EstoqueConnection")
            ?? "Server=localhost;Port=3306;Database=estoque_db;Uid=root;Pwd=;";

        var optionsBuilder = new DbContextOptionsBuilder<EstoqueDbContext>();
        
        // Usa versão fixa do MySQL para design-time (não precisa conectar)
        var serverVersion = ServerVersion.Parse("8.0.0-mysql");
        
        optionsBuilder.UseMySql(
            connectionString,
            serverVersion,
            mySqlOptions => mySqlOptions
                .EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null
                )
                .MigrationsAssembly("Estoque.Infrastructure")
        );

        return new EstoqueDbContext(optionsBuilder.Options);
    }
}