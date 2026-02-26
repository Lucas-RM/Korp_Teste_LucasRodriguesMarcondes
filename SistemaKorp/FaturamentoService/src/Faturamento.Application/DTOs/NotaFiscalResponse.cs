namespace Faturamento.Application.DTOs;

public class NotaFiscalResponse
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ItemNotaFiscalResponseDto> Itens { get; set; } = new();
    public DateTime CriadoEm { get; set; }
    public DateTime? FechadoEm { get; set; }
}

