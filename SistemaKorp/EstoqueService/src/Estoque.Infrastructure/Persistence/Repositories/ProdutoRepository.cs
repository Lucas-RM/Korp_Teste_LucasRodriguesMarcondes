using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Persistence.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<Produto?> ObterPorCodigoAsync(string codigo)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<IEnumerable<Produto>> ListarTodosAsync()
    {
        return await _context.Produtos.ToListAsync();
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
    }

    public Task AtualizarAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public async Task RemoverAsync(Guid id)
    {
        var produto = await ObterPorIdAsync(id);
        if (produto != null)
            _context.Produtos.Remove(produto);
    }
}

