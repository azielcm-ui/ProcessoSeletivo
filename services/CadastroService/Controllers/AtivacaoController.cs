using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cadastro.Infrastructure;

namespace Cadastro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtivacaoController : ControllerBase
    {
        private readonly IFuncionarioRepository _repo;
        public AtivacaoController(IFuncionarioRepository repo) => _repo = repo;

        [HttpPost("{id}")]
        public async Task<IActionResult> Ativar(Guid id)
        {
            var f = await _repo.GetById(id);
            if (f == null) return NotFound();
            if (f.Ativo) return Ok(new { message = "Already active" });
            f.Ativo = true;
            await _repo.UpdateAsync(f);
            return Ok(new { message = "Activated" });
        }
    }
}
