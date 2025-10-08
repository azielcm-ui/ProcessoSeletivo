using Cadastro.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CadastroService.Infrastructure.Data;

public class AppDbContext__ : IdentityDbContext<ApplicationUser>
{
    public AppDbContext__(DbContextOptions<AppDbContext__> options) : base(options) { }

    public DbSet<Funcionarios_> Funcionarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Funcionarios_>().HasIndex(f => f.Setor);
        builder.Entity<Funcionarios_>().HasIndex(f => f.DataInicio);
    }
}
