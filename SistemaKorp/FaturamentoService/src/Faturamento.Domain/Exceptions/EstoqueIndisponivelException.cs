namespace Faturamento.Domain.Exceptions;

public class EstoqueIndisponivelException : DomainException
{
    public EstoqueIndisponivelException(string message) : base(message)
    {
    }

    public EstoqueIndisponivelException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

