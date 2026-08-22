using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
    {
        public void Configure(EntityTypeBuilder<Orcamento> builder)
        {
            builder.ToTable("Orcamento");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrdemServicoId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.ValorServicos)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.ValorPecas)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.GeradoEm)
                .IsRequired();

            builder.Property(x => x.JustificativaReprovacao)
                .HasColumnType("varchar(500)");

            builder.Ignore(x => x.ValorTotal);
        }
    }
}
