using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using SimpleBankSystem.API.Models;
using BankSystem.Server.Models;
using BankSystem.Server.DB;
using BankSystem.API.Models.Request;

namespace BankService.DAL.Transport.RabbitMq
{
    public class RabbitMqStart : IHostedService
    {
        const string TOPIC_NAME_CREATE_RPC = "rpc_create";
        const string TOPIC_NAME_CLIENT_ID_RPC = "rpc_client_id";
        const string TOPIC_NAME_REQUEST_ID_RPC = "rpc_request_id";
        const string TOPIC_NAME_CREATE = "create";
        const string TOPIC_NAME_CLIENT_ID = "client_id";
        const string TOPIC_NAME_REQUEST_ID = "request_id";

        private DapperBoardGameRepository _dapper;

        public RabbitMqStart(DapperBoardGameRepository dapper)
        {
            _dapper = dapper;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory { Uri = new Uri($"amqp://guest:guest@{rabbitMqHost}:5672") };
            IConnection connection = null;
            for (int i = 0; i < 60; i++)
            {
                try
                {
                    connection = await factory.CreateConnectionAsync();
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: "Main", type: ExchangeType.Topic);
            await DeclareCreate(channel);
            await DeclareGetByClientId(channel);
            await DeclareGetById(channel);
            while (true)
            {
                await Task.Delay(100000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        async Task DeclareGetByClientId(IChannel channel)
        {

            var result = await channel.QueueDeclareAsync(durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            var requestQue = result.QueueName;
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            await channel.QueueBindAsync(queue: requestQue, exchange: "Main", routingKey: TOPIC_NAME_CLIENT_ID);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
                IChannel ch = cons.Channel;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                IReadOnlyBasicProperties props = ea.BasicProperties;
                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId,
                };

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var objquery = JsonConvert.DeserializeObject<GetTransactionStateQuery>(message);
                    var dbResult = await _dapper.GetByClient(objquery.ClientId, objquery.DepartmentAddress);
                    var result = dbResult.Select(obj => new GetTransactionStateResult()
                    {
                        ClientId = objquery.ClientId,
                        DepartmentAddress = objquery.DepartmentAddress,
                        State = obj
                    }).ToList();
                    var objResult = JsonConvert.SerializeObject(result);
                    response = objResult;
                }
                catch (Exception e)
                {
                    Console.WriteLine($" [.] {e.Message}");
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await ch.BasicPublishAsync(exchange: "Main", routingKey: TOPIC_NAME_CLIENT_ID_RPC,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);
                    await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await channel.BasicConsumeAsync(requestQue, false, consumer);
        }

        async Task DeclareGetById(IChannel channel)
        {

            var result = await channel.QueueDeclareAsync(durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            var requestQue = result.QueueName;
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            await channel.QueueBindAsync(queue: requestQue, exchange: "Main", routingKey: TOPIC_NAME_REQUEST_ID);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
                IChannel ch = cons.Channel;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                IReadOnlyBasicProperties props = ea.BasicProperties;
                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId,
                };

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var objquery = JsonConvert.DeserializeObject<int>(message);
                    var dbResult = await _dapper.Get(objquery);
                    var result = new GetTransactionStateByIdResult()
                    {
                        RequestId = objquery,
                        State = dbResult
                    };
                    var objResult = JsonConvert.SerializeObject(result);
                    response = objResult;
                }
                catch (Exception e)
                {
                    Console.WriteLine($" [.] {e.Message}");
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await ch.BasicPublishAsync(exchange: "Main", routingKey: TOPIC_NAME_REQUEST_ID_RPC,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);
                    await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await channel.BasicConsumeAsync(requestQue, false, consumer);
        }

        async Task DeclareCreate(IChannel channel)
        {

            var result = await channel.QueueDeclareAsync(durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            var requestQue = result.QueueName;
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            await channel.QueueBindAsync(queue: requestQue, exchange: "Main", routingKey: TOPIC_NAME_CREATE);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
                IChannel ch = cons.Channel;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                IReadOnlyBasicProperties props = ea.BasicProperties;
                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId,
                };

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var objquery = JsonConvert.DeserializeObject<CreateTransactionQuery>(message);
                    await _dapper.Add(new Transaction()
                    {
                        ClientId = objquery.ClientId,
                        Currency = objquery.Currency,
                        Amount = objquery.Amount,
                        DepartmentAddress = objquery.DepartmentAddress,
                        State = "Pending"
                    });
                    var result = new CreateTransactionResult()
                    {
                        ClientId = objquery.ClientId,
                        DepartmentAddress = objquery.DepartmentAddress,
                        Currency = objquery.Currency,
                        Amount = objquery.Amount
                    };
                    var objResult = JsonConvert.SerializeObject(result);
                    response = objResult;
                }
                catch (Exception e)
                {
                    Console.WriteLine($" [.] {e.Message}");
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await ch.BasicPublishAsync(exchange: "Main", routingKey: TOPIC_NAME_CREATE_RPC,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);
                    await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await channel.BasicConsumeAsync(requestQue, false, consumer);
        }
    }
}
