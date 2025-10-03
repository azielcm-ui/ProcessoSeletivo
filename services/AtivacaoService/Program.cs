using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
builder.Services.AddHttpClient();
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("ativacao");
var factory = new ConnectionFactory() { HostName = builder.Configuration.GetValue<string>("RABBITMQ__HOST") ?? "rabbitmq" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.QueueDeclare(queue: "ativacao.lote", durable: true, exclusive: false, autoDelete: false);

var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
var cadastroBase = builder.Configuration.GetValue<string>("CADASTRO__BASEURL") ?? "http://cadastro:80";

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var text = Encoding.UTF8.GetString(body);
        var ids = JsonSerializer.Deserialize<Guid[]>(text);
        if (ids == null) { channel.BasicAck(ea.DeliveryTag, false); return; }

        var client = httpClientFactory.CreateClient();
        foreach (var id in ids)
        {
            try
            {
                var resp = await client.PostAsync($"{cadastroBase}/api/ativacao/{id}", null);
                if (!resp.IsSuccessStatusCode) logger.LogWarning("Failed to activate {Id} status: {Status}", id, resp.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error activating {Id}", id);
            }
        }

        channel.BasicAck(ea.DeliveryTag, false);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed processing message");
        channel.BasicNack(ea.DeliveryTag, false, true);
    }
};

channel.BasicConsume(queue: "ativacao.lote", autoAck: false, consumer: consumer);

app.MapGet("/health", () => Results.Ok("OK"));
app.Run();
