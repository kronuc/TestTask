using BankService.DAL.RabbitMq;
using BankSystem.Server.DB;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ConnectionKeeper>();
builder.Services.AddHostedService<DapperAdditionalDbOperations>();
builder.Services.AddSingleton<DapperBoardGameRepository>();
builder.Services.AddHostedService<RabbitMqStart>();
var app = builder.Build();

app.Run();