using GrpcTest.WebClient;

namespace GrpsTest.WebClient
{
    public interface IGreeterGrpcService
    {
        Task<string> SayHello(string name, CancellationToken cancellationToken);
    }

    public class GreeterGrpcService : IGreeterGrpcService
    {
        private readonly Greeter.GreeterClient _client;
        public GreeterGrpcService(Greeter.GreeterClient client)
        {
            _client = client;
        }

        public async Task<string> SayHello(string name, CancellationToken cancellationToken)
        {
            var request = new HelloRequest { Name = name };
            var response = await _client.SayHelloAsync(request, cancellationToken: cancellationToken);
            return response.Message;
        }
    }
}
