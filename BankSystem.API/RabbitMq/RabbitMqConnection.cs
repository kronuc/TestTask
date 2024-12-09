using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;

namespace BankSystem.API.RabbitMq
{
    public class RabbitMqConnection
        : IHostedService, IAsyncDisposable
    {
        private const string TOPIC_NAME_CREATE_RPC = "rpc_create";
        private const string TOPIC_NAME_CLIENT_ID_RPC = "rpc_client_id";
        private const string TOPIC_NAME_REQUEST_ID_RPC = "rpc_request_id";
        private const string TOPIC_NAME_CREATE = "create";
        private const string TOPIC_NAME_CLIENT_ID = "client_id";
        private const string TOPIC_NAME_REQUEST_ID= "request_id";

        private readonly IConnectionFactory _connectionFactory;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
            = new();

        private IConnection? _connection;
        private IChannel? _channel;
        private string? _replyQueueNameCreate;
        private string? _replyQueueNameGetById;
        private string? _replyQueueNameGetByClientId;

        public RabbitMqConnection()
        {
            var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            _connectionFactory = new ConnectionFactory { Uri = new Uri($"amqp://guest:guest@{rabbitMqHost}:5672") };
        }

        public async Task HandleResponce(object model, BasicDeliverEventArgs ea)
        {
            string? correlationId = ea.BasicProperties.CorrelationId;

            if (!string.IsNullOrEmpty(correlationId) && _callbackMapper.TryRemove(correlationId, out var tcs))
            {
                var body = ea.Body.ToArray();
                var stringResponce = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(stringResponce);
            }
        }
        public async Task DeclareCreate()
        {
            QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync();
            _replyQueueNameCreate = queueDeclareResult.QueueName;
            await _channel.QueueBindAsync(queue: _replyQueueNameCreate, exchange: "Main", routingKey: TOPIC_NAME_CREATE_RPC);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleResponce;

            await _channel.BasicConsumeAsync(_replyQueueNameCreate, true, consumer);
        }

        public async Task DeclareGetById()
        {
            QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync();
            _replyQueueNameGetById = queueDeclareResult.QueueName;
            await _channel.QueueBindAsync(queue: _replyQueueNameGetById, exchange: "Main", routingKey: TOPIC_NAME_REQUEST_ID_RPC);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleResponce;
            await _channel.BasicConsumeAsync(_replyQueueNameGetById, true, consumer);
        }

        public async Task DeclareGet()
        {
            QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync();
            _replyQueueNameGetByClientId = queueDeclareResult.QueueName;
            await _channel.QueueBindAsync(queue: _replyQueueNameGetByClientId, exchange: "Main", routingKey: TOPIC_NAME_CLIENT_ID_RPC);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleResponce;

            await _channel.BasicConsumeAsync(_replyQueueNameGetByClientId, true, consumer);
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = await CreateConnection();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "Main", type: ExchangeType.Topic);
            
            await DeclareCreate();
            await DeclareGet();
            await DeclareGetById();
        }

        public async Task<string> CallCreateAsync(string message,
            CancellationToken cancellationToken = default)
        {
            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = _replyQueueNameCreate
            };

            var tcs = new TaskCompletionSource<string>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "Main", routingKey: TOPIC_NAME_CREATE,
                mandatory: true, basicProperties: props, body: messageBytes);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });

            return await tcs.Task;
        }

        public async Task<string> CallGetAsync(string message,
            CancellationToken cancellationToken = default)
        {
            if (_channel is null)
            {
                throw new InvalidOperationException();
            }

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = _replyQueueNameGetByClientId
            };

            var tcs = new TaskCompletionSource<string>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "Main", routingKey: TOPIC_NAME_CLIENT_ID,
                mandatory: true, basicProperties: props, body: messageBytes);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });

            return await tcs.Task;
        }
        public async Task<string> CallGetByIdAsync(string message,
            CancellationToken cancellationToken = default)
        {
            if (_channel is null)
            {
                throw new InvalidOperationException();
            }

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = _replyQueueNameGetByClientId
            };

            var tcs = new TaskCompletionSource<string>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "Main", routingKey: TOPIC_NAME_REQUEST_ID,
                mandatory: true, basicProperties: props, body: messageBytes);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });

            return await tcs.Task;
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
