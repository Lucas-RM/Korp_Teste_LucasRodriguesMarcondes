namespace Faturamento.Domain.Exceptions;

public class NotaFiscalJaFechadaException : DomainException
{
    public NotaFiscalJaFechadaException(int numero)
        : base($"Nota fiscal {numero} já foi fechada e não pode ser modificada.")
    {
    }
}

