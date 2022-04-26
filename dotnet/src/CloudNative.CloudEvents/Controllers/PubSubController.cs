using CloudEvents.Services;
using CloudNative.CloudEvents;
using DotnetApplication;
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
        var messages = new List<CloudEvent>
        {
            new()
            {
                Data = new HelloWorldDotNetMessage
                {
                    Message = "Hello World"
                },
                DataContentType = null,
                Id = Guid.NewGuid().ToString(),
                DataSchema = null,
                Source = new Uri("/PubSub/SendMessage"),
                Time = DateTimeOffset.Now,
                Type = typeof(HelloWorldDotNetMessage).FullName
            }
        };

        await _publisher.PublishMessagesAsync(messages);
    }
    
    [HttpGet("ReceiveMessage")]
    public async Task<int> Receive()
    {
        var obj = new HelloWorldDotNetMessage();
        obj.Message = "Hello World";

        return await _publisher.PullMessagesAsync(true, "golang-application-dotnet-subscription");
    }
}
