using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadastro.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Infrastructure
{
    public interface IFuncionarioRepository
    {
        Task AddAsync(Funcionario f);
        Task<List<Funcionario>> ObterPorPeriodo(DateTime? from, DateTime? to);
        Task<Funcionario?> GetById(Guid id);
        Task UpdateAsync(Funcionario f);
        Task<List<Funcionario>> ObterPorDataInicio(DateTime data);
    }

    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly CadastroDbContext _db;
        public FuncionarioRepository(CadastroDbContext db) => _db = db;

        public async Task AddAsync(Funcionario f)
        {
            _db.Funcionarios.Add(f);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Funcionario>> ObterPorPeriodo(DateTime? from, DateTime? to)
        {
            var q = _db.Funcionarios.AsQueryable();
            if (from.HasValue) q = q.Where(f => f.DataInicio >= from.Value);
            if (to.HasValue) q = q.Where(f => f.DataInicio <= to.Value);
            return await q.ToListAsync();
        }

        public async Task<Funcionario?> GetById(Guid id) => await _db.Funcionarios.FindAsync(id);

        public async Task UpdateAsync(Funcionario f)
        {
            _db.Funcionarios.Update(f);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Funcionario>> ObterPorDataInicio(DateTime data)
        {
            var d = data.Date;
            return await _db.Funcionarios.Where(f => f.DataInicio == d && !f.Ativo).ToListAsync();
        }
    }
}
