using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cadastro.Infrastructure;
using Cadastro.Domain;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Cadastro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionariosController : ControllerBase
    {
        private readonly IFuncionarioRepository _repo;
        private readonly IEmailSender _email;
        private readonly IRabbitMqPersistentConnection _rabbit;
        private readonly ILogger<FuncionariosController> _logger;

        public FuncionariosController(IFuncionarioRepository repo, IEmailSender email, IRabbitMqPersistentConnection rabbit, ILogger<FuncionariosController> logger)
        {
            _repo = repo;
            _email = email;
            _rabbit = rabbit;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFuncionarioDto dto)
        {
            var f = new Funcionario
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                DataInicio = dto.DataInicio.Date,
                Setor = dto.Setor,
                Ativo = false,
                Email = dto.Email
            };
            await _repo.AddAsync(f);

            // send email (placeholder)
            await _email.SendEmailAsync(f.Email, "Bem-vindo(a)", $"Sua data de início: {f.DataInicio:yyyy-MM-dd}") ;

            // publish event to rabbitmq
            try
            {
                using var model = _rabbit.CreateModel();
                model.QueueDeclare(queue: "funcionario.criado", durable: true, exclusive: false, autoDelete: false);
                var body = JsonSerializer.Serialize(new { Id = f.Id, Nome = f.Nome, Setor = f.Setor, DataInicio = f.DataInicio });
                var bytes = System.Text.Encoding.UTF8.GetBytes(body);
                model.BasicPublish(exchange: "", routingKey: "funcionario.criado", basicProperties: null, body: bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish rabbitmq event");
            }

            return CreatedAtAction(nameof(Get), new { id = f.Id }, f);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] bool groupBySetor = false)
        {
            var items = await _repo.ObterPorPeriodo(fromDate, toDate);
            if (!groupBySetor) return Ok(items);
            var grouped = items.GroupBy(i => i.Setor).ToDictionary(g => g.Key, g => g.ToList());
            return Ok(grouped);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var f = await _repo.GetById(id);
            if (f == null) return NotFound();
            return Ok(f);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFuncionarioDto dto)
        {
            var f = await _repo.GetById(id);
            if (f == null) return NotFound();
            f.Nome = dto.Nome ?? f.Nome;
            f.Telefone = dto.Telefone ?? f.Telefone;
            if (dto.DataInicio.HasValue) f.DataInicio = dto.DataInicio.Value.Date;
            f.Setor = dto.Setor ?? f.Setor;
            await _repo.UpdateAsync(f);

            // publish data updated event
            try
            {
                using var model = _rabbit.CreateModel();
                model.QueueDeclare(queue: "funcionario.atualizado", durable: true, exclusive: false, autoDelete: false);
                var body = JsonSerializer.Serialize(new { Id = f.Id, Nome = f.Nome, Setor = f.Setor, DataInicio = f.DataInicio });
                var bytes = System.Text.Encoding.UTF8.GetBytes(body);
                model.BasicPublish(exchange: "", routingKey: "funcionario.atualizado", basicProperties: null, body: bytes);
            }
            catch (Exception) { }

            return NoContent();
        }
    }

    public record CreateFuncionarioDto(string Nome, string Telefone, DateTime DataInicio, string Setor, string Email);
    public record UpdateFuncionarioDto(string? Nome, string? Telefone, DateTime? DataInicio, string? Setor);
}
