using BankSystem.API.Models.Request;
using BankSystem.API.Transport.Transport;
using SimpleBankSystem.API.Models;

namespace BankSystem.API.Transport.Grpc.Services
{
    public class TransactionGrpcService : ITransactionTransportService
    {
        private readonly CreateTransactionService.CreateTransactionServiceClient _client;
        public TransactionGrpcService(CreateTransactionService.CreateTransactionServiceClient client)
        {
            _client = client;
        }

        public async Task<CreateTransactionResult> CreateTransaction(CreateTransactionQuery name, CancellationToken cancellationToken)
        {
            var request = new CreateTransactionQueryGrpc { Currency = (int)name.Currency, Amount = name.Amount, ClientId = name.ClientId, DepartmentAddress = name.DepartmentAddress };
            var response = await _client.CreateTransactionAsync(request, cancellationToken: cancellationToken);
            var result = new CreateTransactionResult { ClientId = response.ClientId, DepartmentAddress = response.DepartmentAddress, Amount = response.Amount, Currency = (Currency)response.Currency };
            return result;
        }

        public async Task<IEnumerable<GetTransactionStateResult>> GetState(GetTransactionStateQuery query, CancellationToken cancellationToken)
        {
            var request = new GetTransactionStateQueryGrpc { ClientId = query.ClientId, DepartmentAddress = query.DepartmentAddress };
            return null;
        }

        public Task<GetTransactionStateResult> GetState(string transactionId, CancellationToken cancellationToken)
        {
            var request = new GetTransactionStateByIdQueryGrpc { TrunsactionId = transactionId };
            return null;
        }
    }
}
