using BankSystem.API.Models.Request;
using SimpleBankSystem.API.Models;

namespace BankSystem.API.Transport
{
    public interface ITransactionTransportService
    {
        public Task<CreateTransactionResult> CreateTransaction(CreateTransactionQuery name, CancellationToken cancellationToken);
        public Task<IEnumerable<GetTransactionStateResult>> GetState(GetTransactionStateQuery query, CancellationToken cancellationToken);
        public Task<GetTransactionStateResult> GetState(string transactionId, CancellationToken cancellationToken);
    }
}
