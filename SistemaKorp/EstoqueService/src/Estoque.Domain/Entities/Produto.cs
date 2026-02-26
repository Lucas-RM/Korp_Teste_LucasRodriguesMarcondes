namespace Estoque.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal Saldo { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public bool SimularFalha { get; private set; }

    private Produto() { } // EF Core

    public static Produto Create(string codigo, string descricao, decimal saldo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new Exceptions.DomainException("Código do produto não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new Exceptions.DomainException("Descrição do produto não pode ser vazia.");

        if (codigo.Length > 20)
            throw new Exceptions.DomainException("Código do produto não pode ter mais de 20 caracteres.");

        if (descricao.Length > 200)
            throw new Exceptions.DomainException("Descrição do produto não pode ter mais de 200 caracteres.");

        if (saldo < 0)
            throw new Exceptions.DomainException("Saldo não pode ser negativo.");

        return new Produto
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Descricao = descricao,
            Saldo = saldo,
            CriadoEm = DateTime.UtcNow,
            SimularFalha = false
        };
    }

    public void BaixarSaldo(decimal quantidade)
    {
        if (SimularFalha)
            throw new Exceptions.FalhaSimuladaException(Codigo);

        if (quantidade <= 0)
            throw new Exceptions.DomainException("Quantidade deve ser maior que zero.");

        if (Saldo < quantidade)
            throw new Exceptions.SaldoInsuficienteException(Codigo, Saldo, quantidade);

        Saldo -= quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AtualizarSaldo(decimal novoSaldo)
    {
        if (novoSaldo < 0)
            throw new Exceptions.DomainException("Saldo não pode ser negativo.");

        Saldo = novoSaldo;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AtivarSimulacaoFalha()
    {
        SimularFalha = true;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void DesativarSimulacaoFalha()
    {
        SimularFalha = false;
        AtualizadoEm = DateTime.UtcNow;
    }
}

