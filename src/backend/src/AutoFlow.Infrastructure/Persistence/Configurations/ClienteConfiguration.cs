using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.OwnsOne(cliente => cliente.Documento, documento =>
            {
                documento.Property(d => d.Numero)
                    .HasColumnName("Documento")
                    .IsRequired()
                    .HasColumnType("varchar(14)");
            });

            builder.Property(x => x.Telefone)
                .IsRequired()
                .HasColumnType("varchar(15)");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.ToTable("Cliente");
        }
    }
}
