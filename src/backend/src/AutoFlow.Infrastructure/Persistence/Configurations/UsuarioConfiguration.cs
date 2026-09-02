using AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoFlow.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(u => u.Perfil)
                .HasConversion<string>()
                .IsRequired()
                .HasColumnType("varchar(20)");

            builder.OwnsOne(usuario => usuario.Email, email =>
            {
                email.Property(e => e.Endereco)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                email.HasIndex(e => e.Endereco).IsUnique();
            });

            builder.OwnsOne(usuario => usuario.Senha, senha =>
            {
                senha.Property(s => s.Hash)
                    .HasColumnName("SenhaHash")
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                senha.Property(s => s.Salt)
                    .HasColumnName("SenhaSalt")
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(u => u.ClienteId)
                .IsRequired(false);

            builder.ToTable("Usuario");
        }
    }
}
