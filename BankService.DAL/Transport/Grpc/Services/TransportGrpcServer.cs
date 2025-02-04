using BankSystem.DAL;
using Grpc.Core;

namespace BankService.DAL.Transport.Grpc.Services
{
    public class TransportGrpcServer : CreateTransactionService.CreateTransactionServiceBase
    {
        public TransportGrpcServer() { }

        public override Task<CreateTransactionResponceGrpc> CreateTransaction(CreateTransactionQueryGrpc request, ServerCallContext context)
        {
            Console.WriteLine("Hello " + request.DepartmentAddress);
            return Task.FromResult(new CreateTransactionResponceGrpc() { Amount = request.Amount, ClientId = request.ClientId, Currency = request.Currency, DepartmentAddress = request.DepartmentAddress });
        }
    }
}
