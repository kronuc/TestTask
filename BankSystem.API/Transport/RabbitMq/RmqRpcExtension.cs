using BankSystem.API.Transport.Abstraction;
using BankSystem.API.Transport.RabbitMq.Base;
using BankSystem.API.Transport.RabbitMq.Base.RmqConnection;
using BankSystem.API.Transport.RabbitMq.Services;

namespace BankSystem.API.Transport.RabbitMq
{
    public static class RmqRpcExtension
    {
        public static void AddRmqRpc(this IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("TRANSPORT") != "rmq") return;
            services.AddSingleton<RmqConnectionKeeper>();
            services.AddHostedService<RmqConnectionKeeper>(provider => provider.GetService<RmqConnectionKeeper>());
            services.AddSingleton<IRmqConnection, RmqRpcConnection>();
            services.AddScoped<ITransactionTransportService, RmqTransactionTransportService>();
        }
    }
}
