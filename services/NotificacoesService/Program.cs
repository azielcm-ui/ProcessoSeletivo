using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddLogging();
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("OK"));

app.MapHub<SetorHub>("/hubs/setor");

var factory = new ConnectionFactory() { HostName = builder.Configuration.GetValue<string>("RABBITMQ__HOST") ?? "rabbitmq" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.QueueDeclare(queue: "funcionario.criado", durable: true, exclusive: false, autoDelete: false);
channel.QueueDeclare(queue: "funcionario.atualizado", durable: true, exclusive: false, autoDelete: false);

var scopeFactory = app.Services;
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("notificacoes");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    var text = Encoding.UTF8.GetString(ea.Body.ToArray());
    try
    {
        var payload = JsonSerializer.Deserialize<JsonElement>(text);
        var setor = payload.GetProperty("Setor").GetString();
        var hub = app.Services.GetRequiredService<IHubContext<SetorHub>>();
        await hub.Clients.Group(setor ?? "").SendAsync("FuncionarioNovo", payload);
        channel.BasicAck(ea.DeliveryTag, false);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed handling notificacao");
        channel.BasicAck(ea.DeliveryTag, false);
    }
};

channel.BasicConsume(queue: "funcionario.criado", autoAck: false, consumer: consumer);
channel.BasicConsume(queue: "funcionario.atualizado", autoAck: false, consumer: consumer);

app.Run();

public class SetorHub : Hub
{
    public Task JoinSetor(string setor)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, setor);
    }
}
