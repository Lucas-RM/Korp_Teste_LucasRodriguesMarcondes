using Faturamento.Domain.Entities;

namespace Faturamento.Domain.Interfaces;

public interface INotaFiscalRepository
{
    Task<NotaFiscal?> ObterPorIdAsync(Guid id);
    Task<NotaFiscal?> ObterPorIdComItensAsync(Guid id);
    Task<IEnumerable<NotaFiscal>> ListarTodosAsync();
    Task AdicionarAsync(NotaFiscal nota);
    Task AtualizarAsync(NotaFiscal nota);
}

