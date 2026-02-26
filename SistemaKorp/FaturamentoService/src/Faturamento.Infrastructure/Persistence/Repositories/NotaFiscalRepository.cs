using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;
using Faturamento.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure.Persistence.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(Guid id)
    {
        return await _context.NotasFiscais.FindAsync(id);
    }

    public async Task<NotaFiscal?> ObterPorIdComItensAsync(Guid id)
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<NotaFiscal>> ListarTodosAsync()
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .ToListAsync();
    }

    public async Task AdicionarAsync(NotaFiscal nota)
    {
        await _context.NotasFiscais.AddAsync(nota);
    }

    public Task AtualizarAsync(NotaFiscal nota)
    {
        _context.NotasFiscais.Update(nota);
        return Task.CompletedTask;
    }
}

