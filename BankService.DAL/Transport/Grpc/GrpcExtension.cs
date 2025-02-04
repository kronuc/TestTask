using BankService.DAL.Transport.Grpc.Services;

namespace BankService.DAL.Transport.Grpc
{
    public static class GrpcExtension
    {
        public static void AddGrpcConnection(this IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("TRANSPORT") != "grpc") return;
            services.AddGrpc();
        }

        public static void MapGrpcServices(this WebApplication app)
        {
            if(Environment.GetEnvironmentVariable("TRANSPORT") != "grpc") return;
            app.MapGrpcService<TransportGrpcServer>();
        }
    }
}
