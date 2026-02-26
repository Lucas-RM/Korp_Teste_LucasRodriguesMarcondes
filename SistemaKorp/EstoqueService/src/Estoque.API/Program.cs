using Estoque.API.Extensions;
using Estoque.API.Middlewares;
using Estoque.Application.DependencyInjection;
using Estoque.Infrastructure.DependencyInjection;
using Estoque.Infrastructure.Persistence.Context;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Serviços
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}
app.UseHttpsRedirection();
app.MapControllers();

// Aplicar migrations automaticamente (desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Erro ao aplicar migrations");
    }
}

// Exibir informações de inicialização
var urls = builder.Configuration["Urls"] ?? "http://localhost:5001";
Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("Estoque API - Iniciada com sucesso!");
Console.WriteLine(new string('=', 60));
Console.WriteLine($"- URL da API: {urls}");
Console.WriteLine($"- Swagger: {urls}/swagger");
Console.WriteLine(new string('=', 60) + "\n");

app.Run();
