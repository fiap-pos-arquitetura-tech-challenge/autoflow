using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class OrdemServicoItemPecaConfiguration : IEntityTypeConfiguration<OrdemServicoItemPeca>
    {
        public void Configure(EntityTypeBuilder<OrdemServicoItemPeca> builder)
        {
            builder.ToTable("OrdemServicoItemPeca");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrdemServicoId)
                .IsRequired();

            builder.Property(x => x.PecaId)
                .IsRequired();

            builder.Property(x => x.Descricao)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(x => x.Quantidade)
                .IsRequired();

            builder.Property(x => x.ValorUnitario)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Ignore(x => x.Subtotal);
        }
    }
}
