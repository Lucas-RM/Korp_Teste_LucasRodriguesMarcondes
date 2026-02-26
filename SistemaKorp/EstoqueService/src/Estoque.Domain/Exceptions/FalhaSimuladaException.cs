namespace Estoque.Domain.Exceptions;

public class FalhaSimuladaException : DomainException
{
    public FalhaSimuladaException(string codigoProduto)
        : base($"Falha simulada para o produto {codigoProduto}. Esta é uma exceção de teste para resiliência.")
    {
    }
}

