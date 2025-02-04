using BankSystem.API.Transport.Grpc.Services;
using BankSystem.API.Transport.Transport;

namespace BankSystem.API.Transport.Grpc
{
    public static class AddGrpcExtension
    {
        public static void AddGrpcConnection(this IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("TRANSPORT") != "grpc") return;
            services.AddGrpcClient<CreateTransactionService.CreateTransactionServiceClient>(client => client.Address = new Uri("http://localhost:5053"));
            services.AddScoped<ITransactionTransportService, TransactionGrpcService>();
        }
    }
}
