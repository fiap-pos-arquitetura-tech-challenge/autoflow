using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class OrdemServicoItemServicoConfiguration : IEntityTypeConfiguration<OrdemServicoItemServico>
    {
        public void Configure(EntityTypeBuilder<OrdemServicoItemServico> builder)
        {
            builder.ToTable("OrdemServicoItemServico");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrdemServicoId)
                .IsRequired();

            builder.Property(x => x.ServicoId)
                .IsRequired();

            builder.Property(x => x.Descricao)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(x => x.Quantidade)
                .IsRequired();

            builder.Property(x => x.ValorUnitario)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.TempoPrevisto)
                .IsRequired();

            builder.Property(x => x.ExecucaoIniciadaEm);

            builder.Property(x => x.ExecucaoFinalizadaEm);

            builder.Ignore(x => x.Subtotal);
        }
    }
}
