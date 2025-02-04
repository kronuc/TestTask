using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;

namespace BankSystem.API.Transport.RabbitMq.Base
{
    public class RmqRpcBuilder : IHostedService
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
            = new();
        private string _replyQueueName;
        private RmqConnectionKeeper _connectionKeeper;
        private string _topic;
        private string _rpcTopic;
        public RmqRpcBuilder(RmqConnectionKeeper connectionKeeper, string topic, string rpcTopic)
        {
            _topic = topic;
            _rpcTopic = rpcTopic;
            _connectionKeeper = connectionKeeper;
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
        public async Task<string> GetState(string message,
            CancellationToken cancellationToken)
        {
            var channel = await _connectionKeeper.GetChannelAsync();
            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = _replyQueueName
            };

            var tcs = new TaskCompletionSource<string>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: "Main", routingKey: _topic,
                mandatory: true, basicProperties: props, body: messageBytes);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });
            return await tcs.Task;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string[] topics = new string[5];
            foreach(var topic in topics)
            {
                var channel = await _connectionKeeper.GetChannelAsync();
                QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
                var replyQueueName = queueDeclareResult.QueueName;
                await channel.QueueBindAsync(queue: _replyQueueName, exchange: "Main", routingKey: topic);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += HandleResponce;

                await channel.BasicConsumeAsync(_replyQueueName, true, consumer);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
