using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<Produto?> ObterPorCodigoAsync(string codigo);
    Task<IEnumerable<Produto>> ListarTodosAsync();
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(Guid id);
}

