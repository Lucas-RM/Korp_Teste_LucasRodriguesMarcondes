namespace Estoque.Domain.Exceptions;

public class SaldoInsuficienteException : DomainException
{
    public SaldoInsuficienteException(string codigoProduto, decimal saldoAtual, decimal quantidadeSolicitada)
        : base($"Saldo insuficiente para o produto {codigoProduto}. Saldo atual: {saldoAtual}, Quantidade solicitada: {quantidadeSolicitada}")
    {
    }
}

