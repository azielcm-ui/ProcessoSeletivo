# Microsserviços .NET 7.0

Conteúdo:
- services/CadastroService
- services/AtivacaoService
- services/NotificacoesService
- deploy/docker-compose.yml
- .env

Este projeto contém:
- CadastroService:  
Responsabilidade: Gerenciar o ciclo de vida dos funcionários (CRUD), expor uma API REST, lidar com a persistência de dados, autenticação de usuários (JWT) e agendar a ativação.
Tecnologias: .NET 7, ASP.NET Core, EF Core, PostgreSQL, Identity, JWT, Hangfire, MediatR, envio de e-mail placeholder
- AtivacaoService: 
Responsabilidade: Consumir eventos de uma fila para ativar funcionários na data de início correta. O processamento é feito em lotes para lidar com alta carga.
Tecnologias:consumidor RabbitMQ que processa lotes e faz chamada HTTP para ativar funcionário.
- NotificacoesService: 
Responsabilidade: Enviar notificações em tempo real para os clientes (front-end dos setores) sobre novos funcionários ou atualizações.
Tecnologias:SignalR hub + consumidor RabbitMQ que repassa eventos para grupos por setor.

**Como usar**
1. Copie `deploy/.env` para `deploy/.env` e ajuste a chave JWT.
2. Execute `docker-compose up --build` no diretório `deploy`.
3. Os serviços aplicam migrations e seeds ao iniciar (o código contém chamadas de initialização).
4. A API do Cadastro estará exposta na porta 5000, Notificações em 5002, Ativação em 5001 (conforme compose).

Para produção, preencha configuração de SMTP, segredos JWT, harden Security.

