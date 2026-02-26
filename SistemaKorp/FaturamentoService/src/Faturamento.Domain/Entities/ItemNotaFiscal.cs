namespace Faturamento.Domain.Entities;

public class ItemNotaFiscal
{
    public Guid Id { get; private set; }
    public Guid NotaFiscalId { get; private set; }
    public string CodigoProduto { get; private set; } = string.Empty;
    public string DescricaoProduto { get; private set; } = string.Empty;
    public decimal Quantidade { get; private set; }
    public NotaFiscal NotaFiscal { get; private set; } = null!;

    private ItemNotaFiscal() { } // EF Core

    public static ItemNotaFiscal Create(Guid notaFiscalId, string codigoProduto, string descricaoProduto, decimal quantidade)
    {
        if (string.IsNullOrWhiteSpace(codigoProduto))
            throw new Exceptions.DomainException("Código do produto não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(descricaoProduto))
            throw new Exceptions.DomainException("Descrição do produto não pode ser vazia.");

        if (codigoProduto.Length > 20)
            throw new Exceptions.DomainException("Código do produto não pode ter mais de 20 caracteres.");

        if (descricaoProduto.Length > 200)
            throw new Exceptions.DomainException("Descrição do produto não pode ter mais de 200 caracteres.");

        if (quantidade <= 0)
            throw new Exceptions.DomainException("Quantidade deve ser maior que zero.");

        return new ItemNotaFiscal
        {
            Id = Guid.NewGuid(),
            NotaFiscalId = notaFiscalId,
            CodigoProduto = codigoProduto,
            DescricaoProduto = descricaoProduto,
            Quantidade = quantidade
        };
    }
}

