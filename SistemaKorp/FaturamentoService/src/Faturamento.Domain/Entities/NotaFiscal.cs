using Faturamento.Domain.Enums;

namespace Faturamento.Domain.Entities;

public class NotaFiscal
{
    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public StatusNotaFiscal Status { get; private set; }
    public List<ItemNotaFiscal> Itens { get; private set; } = new();
    public DateTime CriadoEm { get; private set; }
    public DateTime? FechadoEm { get; private set; }

    private NotaFiscal() { } // EF Core

    public static NotaFiscal Create()
    {
        return new NotaFiscal
        {
            Id = Guid.NewGuid(),
            Status = StatusNotaFiscal.Aberta,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void AdicionarItem(string codigoProduto, string descricaoProduto, decimal quantidade)
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new Exceptions.DomainException("Não é possível adicionar itens a uma nota fiscal fechada.");

        var item = ItemNotaFiscal.Create(Id, codigoProduto, descricaoProduto, quantidade);
        Itens.Add(item);
    }

    public void Fechar()
    {
        if (Status == StatusNotaFiscal.Fechada)
            throw new Exceptions.NotaFiscalJaFechadaException(Numero);

        Status = StatusNotaFiscal.Fechada;
        FechadoEm = DateTime.UtcNow;
    }

    public bool PodeSerImpressao()
    {
        return Status == StatusNotaFiscal.Aberta;
    }
}

