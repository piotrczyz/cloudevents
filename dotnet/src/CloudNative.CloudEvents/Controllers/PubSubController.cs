using CloudEvents.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudEvents.Controllers;

[ApiController]
[Route("[controller]")]
public class PubSubController : ControllerBase
{

    private readonly ILogger<PubSubController> _logger;
    private readonly PubSubPublisher _publisher;

    public PubSubController(ILogger<PubSubController> logger, PubSubPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    [HttpGet("SendMessage")]
    public async Task Get()
    {
        await _publisher.PublishMessagesAsync(new []
        {
            "Message sent from .NET application"
        });
    }
    
    [HttpGet("ReceiveMessage")]
    public async Task Receive()
    {
        await _publisher.PullMessagesAsync(true, "golang-application-dotnet-subscription");
    }
}
