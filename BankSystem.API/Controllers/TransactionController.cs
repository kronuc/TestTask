using BankSystem.API.Models.Request;
using BankSystem.API.Transport.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionTransportService _transactionTransportService;
    public TransactionController(ILogger<TransactionController> logger, ITransactionTransportService transactionTransportService)
    {
        _transactionTransportService = transactionTransportService;
        _logger = logger;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateTransaction([FromBody][Required] CreateTransactionQuery query)
    {
        var result = await _transactionTransportService.CreateTransaction(query, CancellationToken.None);
        return new OkObjectResult(result);
    }

    [HttpGet()]
    public async Task<IActionResult> GetState([FromQuery][Required] GetTransactionStateQuery query)
    {
        var result = await _transactionTransportService.GetState(query, CancellationToken.None);
        return new OkObjectResult(result);
    }

    [HttpGet("/{transactionId}")]
    public async Task<IActionResult> GetState([FromRoute][Required] string transactionId)
    {
        var result = await _transactionTransportService.GetState(transactionId, CancellationToken.None);
        return new OkObjectResult(result);
    }
}