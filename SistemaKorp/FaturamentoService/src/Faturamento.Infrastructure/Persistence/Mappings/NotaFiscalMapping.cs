using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faturamento.Infrastructure.Persistence.Mappings;

public class NotaFiscalMapping : IEntityTypeConfiguration<NotaFiscal>
{
    public void Configure(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("notas_fiscais");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnType("uuid");

        builder.Property(n => n.Numero)
            .UseIdentityByDefaultColumn();

        builder.Property(n => n.Status)
            .HasConversion<int>()
            .HasColumnName("status");

        builder.Property(n => n.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(n => n.FechadoEm)
            .HasColumnName("fechado_em");

        builder.HasMany(n => n.Itens)
            .WithOne(i => i.NotaFiscal)
            .HasForeignKey(i => i.NotaFiscalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

