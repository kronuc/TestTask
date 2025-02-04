using BankService.DAL.DB;
using BankService.DAL.Transport.Grpc;
using BankService.DAL.Transport.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbConnection();

builder.Services.AddRmqConnection();

builder.Services.AddGrpcConnection();

var app = builder.Build();

app.MapGrpcServices();

app.Run();