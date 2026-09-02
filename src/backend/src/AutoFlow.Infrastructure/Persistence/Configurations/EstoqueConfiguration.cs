using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations;

public class EstoqueConfiguration
    : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.OwnsOne(e => e.Quantidade, quantidade =>
        {
            quantidade.Property(q => q.Valor)
                .HasColumnName("Quantidade")
                .IsRequired();
        });

        builder.HasOne(e => e.PecaInsumo)
            .WithOne()
            .HasForeignKey<Estoque>("PecaInsumoId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("PecaInsumoId")
            .IsUnique();

        builder.ToTable("Estoque");
    }
}