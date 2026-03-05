using System;
using CandidatoLar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CandidatoLar.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CandidatoLar.Domain.Entities.Pessoa", b =>
                {
                    b.Property<Guid>("Id").HasColumnType("uniqueidentifier");
                    b.Property<bool>("Ativo").ValueGeneratedOnAdd().HasColumnType("bit").HasDefaultValue(true);
                    b.Property<DateTime>("DataNascimento").HasColumnType("date");
                    b.Property<string>("Nome").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
                    b.HasKey("Id");
                    b.ToTable("Pessoas");

                    b.OwnsOne("CandidatoLar.Domain.ValueObjects.Cpf", "Cpf", b1 =>
                        {
                            b1.Property<Guid>("PessoaId").HasColumnType("uniqueidentifier");
                            b1.Property<string>("Value").IsRequired().HasMaxLength(11).IsFixedLength()
                                .HasColumnType("nchar(11)").HasColumnName("Cpf");
                            b1.HasKey("PessoaId");
                            b1.HasIndex("Value").IsUnique().HasDatabaseName("IX_Pessoas_Cpf");
                            b1.ToTable("Pessoas");
                            b1.WithOwner().HasForeignKey("PessoaId");
                        });

                    b.Navigation("Cpf").IsRequired();
                });

            modelBuilder.Entity("CandidatoLar.Domain.Entities.Telefone", b =>
                {
                    b.Property<Guid>("Id").HasColumnType("uniqueidentifier");
                    b.Property<string>("Numero").IsRequired().HasMaxLength(13).HasColumnType("nvarchar(13)");
                    b.Property<Guid>("PessoaId").HasColumnType("uniqueidentifier");
                    b.Property<int>("Tipo").HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("PessoaId", "Tipo", "Numero").HasDatabaseName("IX_Telefones_PessoaId_Tipo_Numero");
                    b.ToTable("Telefones");
                });

            modelBuilder.Entity("CandidatoLar.Domain.Entities.Telefone", b =>
                {
                    b.HasOne("CandidatoLar.Domain.Entities.Pessoa", null)
                        .WithMany("Telefones")
                        .HasForeignKey("PessoaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
