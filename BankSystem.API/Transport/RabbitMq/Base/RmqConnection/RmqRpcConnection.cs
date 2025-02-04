using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace BankSystem.API.Transport.RabbitMq.Base.RmqConnection
{
    public class RmqRpcConnection : IRmqConnection
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
            = new();
        private RmqConnectionKeeper _connectionKeeper;
        private readonly ConcurrentDictionary<string, string> _responsQueues; // add dependence from exchange
        public RmqRpcConnection(RmqConnectionKeeper connectionKeeper)
        {
            _connectionKeeper = connectionKeeper;
            _responsQueues = new ConcurrentDictionary<string, string>();
        }

        private async Task DeclareRpcResponceQueue(string topicName, string exchange)
        {
            var channel = await _connectionKeeper.GetChannelAsync();
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            var responceQueue = _responsQueues.GetOrAdd(topicName, queueDeclareResult.QueueName);
            _responsQueues[topicName] = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(queue: _responsQueues[topicName], exchange: exchange, routingKey: topicName);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += HandleResponce;

            await channel.BasicConsumeAsync(responceQueue, true, consumer);
        }

        public async Task<string> GetReplieQueueForTopic(string topicName, string exchange)
        {
            if (!_responsQueues.ContainsKey(topicName))
            {
                await DeclareRpcResponceQueue(topicName, exchange);
            }
            return _responsQueues[topicName];
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

        public async Task<string> SendMessageAsync(string message, string exchange, string routingKey, string returnRoutingKey, CancellationToken cancellationToken)
        {
            var channel = await _connectionKeeper.GetChannelAsync();

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = await GetReplieQueueForTopic(returnRoutingKey, exchange),
            };

            var tcs = new TaskCompletionSource<string>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: exchange, routingKey: routingKey,
                mandatory: true, basicProperties: props, body: messageBytes);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });
            return await tcs.Task;
        }
    }
}
