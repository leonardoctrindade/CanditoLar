using CandidatoLar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CandidatoLar.Infrastructure.Persistence.Configurations;

internal sealed class TelefoneConfiguration : IEntityTypeConfiguration<Telefone>
{
    public void Configure(EntityTypeBuilder<Telefone> builder)
    {
        builder.ToTable("Telefones");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.PessoaId)
            .IsRequired();

        builder.Property(t => t.Tipo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Numero)
            .IsRequired()
            .HasMaxLength(13);

        builder.HasIndex(t => new { t.PessoaId, t.Tipo, t.Numero })
            .HasDatabaseName("IX_Telefones_PessoaId_Tipo_Numero");
    }
}
