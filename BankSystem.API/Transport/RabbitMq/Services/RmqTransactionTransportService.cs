using BankSystem.API.Models.Request;
using BankSystem.API.Transport.Abstraction;
using BankSystem.API.Transport.RabbitMq.Base.RmqConnection;
using Newtonsoft.Json;
using SimpleBankSystem.API.Models;
using System.Xml.Linq;

namespace BankSystem.API.Transport.RabbitMq.Services
{
    public class RmqTransactionTransportService : ITransactionTransportService
    {
        private readonly IRmqConnection _connection;
        private readonly string _exchange;
        private readonly string _createTopic;
        private readonly string _requestIdTopic;
        private readonly string _clientIdTopic;
        private readonly string _createRpcTopic;
        private readonly string _requestIdRpcTopic;
        private readonly string _clientIdRpcTopic;

        public RmqTransactionTransportService(IRmqConnection connection, IConfiguration configuration)
        {
            _connection = connection;

            var rmqConfig = configuration.GetSection("RmqConfig:Exchanges:Main");
            _exchange = rmqConfig.GetValue<string>("Name");
            var topicSerction = rmqConfig.GetSection("Topics");
            _createTopic = topicSerction.GetValue<string>("Create");
            _requestIdTopic = topicSerction.GetValue<string>("RequestId");
            _clientIdTopic = topicSerction.GetValue<string>("ClientId");
            _createRpcTopic = topicSerction.GetValue<string>("CreateRpc");
            _requestIdRpcTopic = topicSerction.GetValue<string>("RequestIdRpc");
            _clientIdRpcTopic = topicSerction.GetValue<string>("ClientIdRpc");
        }

        public async Task<CreateTransactionResult> CreateTransaction(CreateTransactionQuery name, CancellationToken cancellationToken)
        {
            var stringResult = await _connection.SendMessageAsync(JsonConvert.SerializeObject(name), _exchange, _createTopic, _createRpcTopic, cancellationToken);
            return JsonConvert.DeserializeObject<CreateTransactionResult>(stringResult);
        }

        public async Task<IEnumerable<GetTransactionStateResult>> GetState(GetTransactionStateQuery query, CancellationToken cancellationToken)
        {
            var stringResult = await _connection.SendMessageAsync(JsonConvert.SerializeObject(query), _exchange, _clientIdTopic, _clientIdRpcTopic, cancellationToken);
            return JsonConvert.DeserializeObject<IEnumerable<GetTransactionStateResult>>(stringResult);
        }

        public async Task<GetTransactionStateResult> GetState(string transactionId, CancellationToken cancellationToken)
        {
            var stringResult = await _connection.SendMessageAsync(JsonConvert.SerializeObject(transactionId), _exchange, _requestIdTopic, _requestIdRpcTopic, cancellationToken);
            return JsonConvert.DeserializeObject<GetTransactionStateResult>(stringResult);
        }
    }
}
