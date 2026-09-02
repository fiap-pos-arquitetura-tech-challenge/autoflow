using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations;

public class PecaInsumoConfiguration
    : IEntityTypeConfiguration<PecaInsumo>
{
    public void Configure(EntityTypeBuilder<PecaInsumo> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.OwnsOne(p => p.Valor, dinheiro =>
        {
            dinheiro.Property(d => d.Valor)
                .HasColumnName("Valor")
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.ToTable("PecaInsumo");
    }
}