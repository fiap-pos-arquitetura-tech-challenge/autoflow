using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("Veiculos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Marca)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(v => v.Modelo)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(v => v.Cor)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(v => v.AnoFabricacao)
                .IsRequired();

            builder.Property(v => v.AnoModelo)
                .IsRequired();

            builder.Property(v => v.Tipo)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(v => v.Combustivel)
                .HasConversion<string>()
                .IsRequired();

            builder.OwnsOne(v => v.Placa, placa =>
            {
                placa.Property(p => p.Valor)
                    .HasColumnName("Placa")
                    .HasMaxLength(8)
                    .IsRequired();
            });

            builder.OwnsOne(v => v.Chassi, chassi =>
            {
                chassi.Property(c => c.Valor)
                    .HasColumnName("Chassi")
                    .HasMaxLength(17)
                    .IsRequired();
            });

            builder.OwnsOne(v => v.Quilometragem, km =>
            {
                km.Property(q => q.Valor)
                    .HasColumnName("Quilometragem")
                    .IsRequired();
            });

            builder.HasOne(v => v.Cliente)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
