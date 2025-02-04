using System.Runtime.CompilerServices;

namespace BankService.DAL.Transport.RabbitMq
{
    public static class RmqExtension
    {
        public static void AddRmqConnection(this IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("TRANSPORT") != "rmq") return;
            services.AddHostedService<RabbitMqStart>();
        }
    }
}
