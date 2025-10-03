using Microsoft.EntityFrameworkCore;
using Cadastro.Domain;

namespace Cadastro.Infrastructure
{
    public class CadastroDbContext : DbContext
    {
        public CadastroDbContext(DbContextOptions<CadastroDbContext> options) : base(options) { }
        public DbSet<Funcionario> Funcionarios { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Funcionario>(b =>
            {
                b.HasKey(f => f.Id);
                b.Property(f => f.Nome).IsRequired().HasMaxLength(200);
                b.Property(f => f.Telefone).HasMaxLength(30);
                b.Property(f => f.Setor).HasMaxLength(100);
                b.Property(f => f.DataInicio).IsRequired();
                b.Property(f => f.Ativo).IsRequired();
                b.Property(f => f.Email).HasMaxLength(200);
            });
        }
    }
}
