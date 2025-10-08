using System;

namespace Cadastro.Domain
{
    public class Funcionarios_
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public DateTime DataInicio { get; set; }
        public string Setor { get; set; } = null!;
        public bool Ativo { get; set; } = false;
        public string Email { get; set; } = null!;
    }
}
