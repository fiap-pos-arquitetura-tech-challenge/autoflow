using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
    {
        public void Configure(EntityTypeBuilder<OrdemServico> builder)
        {
            builder.ToTable("OrdemServico");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClienteId)
                .IsRequired();

            builder.Property(x => x.VeiculoId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.AvariasObservadas)
                .HasColumnType("varchar(1000)");

            builder.Property(x => x.Diagnostico)
                .HasColumnType("varchar(2000)");

            builder.Property(x => x.DataAbertura)
                .IsRequired();

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Veiculo>()
                .WithMany()
                .HasForeignKey(x => x.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Servicos)
                .WithOne()
                .HasForeignKey(x => x.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Pecas)
                .WithOne()
                .HasForeignKey(x => x.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Orcamento)
                .WithOne()
                .HasForeignKey<Orcamento>(x => x.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
