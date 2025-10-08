using System;
using System.Linq;
using Cadastro.Domain;
using CadastroService.Infrastructure.Data;

namespace Cadastro.Infrastructure
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext__ context)//CadastroDbContext context)
        {
            if (context.Funcionarios.Any()) return;

            var setores = new[] { "TI", "RH", "Financeiro", "Comercial", "Marketing" };
            var hoje = DateTime.UtcNow.Date;
            for (int i = 0; i < 1200; i++)
            {
                var f = new Funcionarios_
                {
                    Nome = $"Funcionario {i}",
                    Telefone = $"+5511999{i:0000}",
                    Email = $"user{i}@example.com",
                    DataInicio = hoje.AddDays(i % 30 - 15),
                    Setor = setores[i % setores.Length],
                    Ativo = false
                };
                context.Funcionarios.Add(f);
            }
            context.SaveChanges();
        }
    }
}
