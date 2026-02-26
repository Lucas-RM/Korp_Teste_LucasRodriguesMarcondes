using Estoque.Domain.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Persistence.Context;

public class EstoqueDbContext : DbContext, IUnitOfWork
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProdutoMapping).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }
}

