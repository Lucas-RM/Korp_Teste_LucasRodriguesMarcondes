using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faturamento.Infrastructure.Persistence.Mappings;

public class ItemNotaFiscalMapping : IEntityTypeConfiguration<ItemNotaFiscal>
{
    public void Configure(EntityTypeBuilder<ItemNotaFiscal> builder)
    {
        builder.ToTable("itens_nota_fiscal");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnType("uuid");

        builder.Property(i => i.NotaFiscalId)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(i => i.CodigoProduto)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.DescricaoProduto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Quantidade)
            .HasColumnType("numeric(18,4)");
    }
}

