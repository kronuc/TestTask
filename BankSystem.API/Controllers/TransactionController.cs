using BankSystem.API.Models.Request;
using BankSystem.API.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleBankSystem.API.Models;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly RabbitMqConnection _rpcClient;
    public TransactionController(ILogger<TransactionController> logger, RabbitMqConnection rpcClient)
    {
        _logger = logger;
        _rpcClient = rpcClient;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateTransaction([FromBody][Required] CreateTransactionQuery query)
    {
        var stringResult = await _rpcClient.CallCreateAsync(JsonConvert.SerializeObject(query));
        var result = JsonConvert.DeserializeObject<CreateTransactionResult>(stringResult);
        return new OkObjectResult(result);
    }

    [HttpGet()]
    public async Task<IActionResult> GetState([FromQuery][Required] GetTransactionStateQuery query)
    {
        var stringResult = await _rpcClient.CallGetAsync(JsonConvert.SerializeObject(query));
        var result = JsonConvert.DeserializeObject<IEnumerable<GetTransactionStateResult>>(stringResult);
        return new OkObjectResult(result);
    }

    [HttpGet("/{transactionId}")]
    public async Task<IActionResult> GetState([FromRoute][Required] string transactionId)
    {
        var stringResult = await _rpcClient.CallGetByIdAsync(transactionId);
        var result = JsonConvert.DeserializeObject<GetTransactionStateByIdResult>(stringResult);
        return new OkObjectResult(result);
    }
}