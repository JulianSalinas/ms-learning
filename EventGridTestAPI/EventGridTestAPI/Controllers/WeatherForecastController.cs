using Azure.Messaging.EventGrid;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;

namespace EventGridTestAPI.Controllers;

[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[Authorize]
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var user = await _graphServiceClient.Me.Request().GetAsync();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost("PostIntoEventGrid")]
    public async Task<ActionResult> PostIntoEventGrid()
    {
        var client = new EventGridPublisherClient(
            new Uri("https://ms-learning-eventgrid-topic.eastus-1.eventgrid.azure.net/api/events"),
            new AzureKeyCredential("CuugMdAQqBtWX1qSRRB3McL4bj328aZn+13aw/TsEcQ="));

        var dataToSend = new
        {
            Name = "Aquiles",
            Nickname = "Pello"
        };

        var gridEvent = new EventGridEvent("ExampleEventSubject", "ExampleEventType", "1.0", dataToSend);

        var response = await client.SendEventAsync(gridEvent);

        return Ok(response.Status);
    }

    [HttpPost("PostIntoEventHubs")]
    public async Task<ActionResult> PostIntoEventHubs()
    {
        var client = new EventGridPublisherClient(
            new Uri("https://ms-learning-eventgrid-topic.eastus-1.eventgrid.azure.net/api/events"),
            new AzureKeyCredential("CuugMdAQqBtWX1qSRRB3McL4bj328aZn+13aw/TsEcQ="));

        var dataToSend = new
        {
            Name = "Aquiles",
            Nickname = "Pello"
        };

        var gridEvent = new EventGridEvent("ExampleEventSubject", "ExampleEventType", "1.0", dataToSend);

        var response = await client.SendEventAsync(gridEvent);

        return Ok(response.Status);
    }
}