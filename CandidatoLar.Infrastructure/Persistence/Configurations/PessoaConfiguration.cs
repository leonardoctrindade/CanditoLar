using CandidatoLar.Domain.Entities;
using CandidatoLar.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CandidatoLar.Infrastructure.Persistence.Configurations;

internal sealed class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.ToTable("Pessoas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.OwnsOne(p => p.Cpf, cpf =>
        {
            cpf.Property(c => c.Value)
                .HasColumnName("Cpf")
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();

            cpf.HasIndex(c => c.Value)
                .IsUnique()
                .HasDatabaseName("IX_Pessoas_Cpf");
        });

        builder.Property(p => p.DataNascimento)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(p => p.Telefones)
            .WithOne()
            .HasForeignKey(t => t.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
