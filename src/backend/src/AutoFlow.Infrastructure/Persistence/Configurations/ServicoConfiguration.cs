using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(x => x.Preco)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.TempoMedio)
                .IsRequired()
                .HasColumnType("int");

            builder.ToTable("Servico");
        }
    }
    
}
