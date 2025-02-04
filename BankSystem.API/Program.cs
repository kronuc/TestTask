using BankSystem.API.Transport.Grpc;
using BankSystem.API.Transport.RabbitMq;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRmqRpc();
builder.Services.AddGrpcConnection();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
