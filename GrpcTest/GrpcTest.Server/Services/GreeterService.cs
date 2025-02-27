using Grpc.Core;
using GrpcTest.Server;

namespace GrpcTest.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private static int i = 0;
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine("Hello " + request.Name);
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + " " + i++
            });
        }
    }
}
