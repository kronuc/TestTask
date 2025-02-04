using RabbitMQ.Client;

namespace BankSystem.API.Transport.RabbitMq.Base
{
    public class RmqConnectionKeeper : IHostedService, IAsyncDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        
        private IConnection? _connection;
        private IChannel? _channel;
        
        public RmqConnectionKeeper()
        {
            var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            _connectionFactory = new ConnectionFactory { Uri = new Uri($"amqp://guest:guest@{rabbitMqHost}:5672") };
        }

        public async Task<IConnection> CreateConnection()
        {
            IConnection connection = null;
            while (connection == null)
            {
                try
                {
                    connection = await _connectionFactory.CreateConnectionAsync();
                }
                catch (Exception) { }

                await Task.Delay(1000);
            }
            return connection;
        }
        public async Task<IConnection> GetConnectionAsync() 
        {
            if(_connection == null || !_connection.IsOpen)
            {
                _connection = await CreateConnection();
            }
            return _connection;

        }

        public async Task<IChannel> GetChannelAsync()
        {
            if (_channel == null || _channel.IsClosed)
            {
                var connection = await GetConnectionAsync();
                _channel = await connection.CreateChannelAsync();
            }
            return _channel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = await CreateConnection();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "Main", type: ExchangeType.Topic);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync();
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync();
            }
        }

        public Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await DisposeAsync();
        }
    }
}
