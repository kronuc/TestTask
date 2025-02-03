using Microsoft.AspNetCore.Mvc;

namespace GrpsTest.WebClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IGreeterGrpcService _greeterGrpcService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IGreeterGrpcService greeterGrpcService)
        {
            _logger = logger;
            _greeterGrpcService = greeterGrpcService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<string> Get([FromQuery] string name)
        {
            var result = await _greeterGrpcService.SayHello(name, CancellationToken.None);
            return result;
        }
    }
}
