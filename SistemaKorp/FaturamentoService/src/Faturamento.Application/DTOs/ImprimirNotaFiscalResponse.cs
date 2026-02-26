namespace Faturamento.Application.DTOs;

public class ImprimirNotaFiscalResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public NotaFiscalResponse? Nota { get; set; }
    public bool EmProcessamento { get; set; }
}

