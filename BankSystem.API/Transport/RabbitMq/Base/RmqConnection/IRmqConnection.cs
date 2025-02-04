namespace BankSystem.API.Transport.RabbitMq.Base.RmqConnection
{
    public interface IRmqConnection
    {
        public Task<string> SendMessageAsync(string message, string exchange, string routingKey, string returnRoutingKey, CancellationToken cancellationToken);
    }
}
