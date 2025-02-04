using BankService.DAL.Transport.Grpc;
using BankService.DAL.Transport.RabbitMq;
using BankSystem.Server.DB;
using GrpcTest.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ConnectionKeeper>();
builder.Services.AddHostedService<DapperAdditionalDbOperations>();
builder.Services.AddSingleton<DapperBoardGameRepository>();
builder.Services.AddHostedService<RabbitMqStart>();
builder.Services.AddGrpcConnection();
var app = builder.Build();
app.MapGrpcServices();
app.Run();