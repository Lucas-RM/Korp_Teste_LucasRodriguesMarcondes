using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Estoque.Infrastructure.Persistence.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnType("char(36)");

        builder.Property(p => p.Codigo)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("codigo");

        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("descricao");

        builder.Property(p => p.Saldo)
            .HasColumnType("decimal(18,4)")
            .HasColumnName("saldo");

        builder.Property(p => p.SimularFalha)
            .HasColumnName("simular_falha")
            .HasDefaultValue(false);

        builder.Property(p => p.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(p => p.AtualizadoEm)
            .HasColumnName("atualizado_em");

        builder.HasIndex(p => p.Codigo)
            .IsUnique();
    }
}

