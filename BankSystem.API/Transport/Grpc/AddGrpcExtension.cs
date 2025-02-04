using BankSystem.API.Transport.Abstraction;
using BankSystem.API.Transport.Grpc.Services;

namespace BankSystem.API.Transport.Grpc
{
    public static class AddGrpcExtension
    {
        public static void AddGrpcConnection(this IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("TRANSPORT") != "grpc") return; 

            services.AddGrpcClient<CreateTransactionService.CreateTransactionServiceClient>(client => client.Address = new Uri("http://bank_system_dal:5053"));
            services.AddScoped<ITransactionTransportService, TransactionGrpcService>();
        }
    }
}
