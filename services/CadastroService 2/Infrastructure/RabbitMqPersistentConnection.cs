//using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Logging;
using System;

namespace Cadastro.Infrastructure
{
    public interface IRabbitMqPersistentConnection
    {
        IModel CreateModel();
    }

    public class RabbitMqPersistentConnection : IRabbitMqPersistentConnection, IDisposable
    {
        private readonly IConnection _connection;
        public RabbitMqPersistentConnection(ConnectionFactory factory)
        {
            _connection = factory.CreateConnection();
        }
        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }

        public void Dispose() => _connection?.Dispose();
    }
}
