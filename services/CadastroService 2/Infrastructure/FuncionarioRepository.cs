using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadastro.Domain;
using CadastroService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Infrastructure
{
    public interface IFuncionarioRepository
    {
        Task AddAsync(Funcionarios_ f);
        Task<List<Funcionarios_>> ObterPorPeriodo(DateTime? from, DateTime? to);
        Task<Funcionarios_?> GetById(Guid id);
        Task UpdateAsync(Funcionarios_ f);
        Task<List<Funcionarios_>> ObterPorDataInicio(DateTime data);
    }

    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly AppDbContext__ _db;//CadastroDbContext _db;
        public FuncionarioRepository(AppDbContext__ db) => _db = db;

        public async Task AddAsync(Funcionarios_ f)
        {
            _db.Funcionarios.Add(f);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Funcionarios_>> ObterPorPeriodo(DateTime? from, DateTime? to)
        {
            var q = _db.Funcionarios.AsQueryable();
            if (from.HasValue) q = q.Where(f => f.DataInicio >= from.Value);
            if (to.HasValue) q = q.Where(f => f.DataInicio <= to.Value);
            return await q.ToListAsync();
        }

        public async Task<Funcionarios_?> GetById(Guid id) => await _db.Funcionarios.FindAsync(id);

        public async Task UpdateAsync(Funcionarios_ f)
        {
            _db.Funcionarios.Update(f);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Funcionarios_>> ObterPorDataInicio(DateTime data)
        {
            var d = data.Date;
            return await _db.Funcionarios.Where(f => f.DataInicio == d && !f.Ativo).ToListAsync();
        }
    }
}
