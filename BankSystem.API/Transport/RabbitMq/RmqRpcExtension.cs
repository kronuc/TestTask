namespace BankSystem.API.Transport.RabbitMq
{
    public static class RmqRpcExtension
    {
        public static void AddRmqRpc(this IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("TRANSPORT") != "rmq") return;
            services.AddSingleton<RabbitMqConnection>();
            services.AddHostedService<RabbitMqConnection>(provider => provider.GetService<RabbitMqConnection>());
        }
    }
}
