using CloudEvents.Services;
using CloudNative.CloudEvents;
using DotnetApplication;
using Google.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Mvc;

namespace CloudEvents.Controllers;

[ApiController]
[Route("[controller]")]
public class PubSubController : ControllerBase
{
    private readonly ILogger<PubSubController> _logger;
    private readonly PubSubPublisher _publisher;
    private readonly PubSubSubscriptionManager _subscriptionManager;

    public PubSubController(ILogger<PubSubController> logger, PubSubPublisher publisher, PubSubSubscriptionManager subscriptionManager)
    {
        _logger = logger;
        _publisher = publisher;
        _subscriptionManager = subscriptionManager;
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
                DataContentType = nameof(HelloWorldDotNetMessage),
                Id = Guid.NewGuid().ToString(),
                DataSchema = null,
                Source = new Uri("https://contribution-api.bcc.no"),
                Time = DateTimeOffset.Now,
                Type = nameof(HelloWorldDotNetMessage)
            }
        };

        await _publisher.PublishMessagesAsync(messages);
    }
    
    [HttpGet("ReceiveMessage")]
    public async Task<int> Receive()
    {
        return await _publisher.PullMessagesAsync(true, "test-sub");
    }

    [HttpPost("Topics/{topicName}")]
    public async Task<string> CreateTopic(string topicName)
    {
        return (await _subscriptionManager.CreateTopic(topicName)).Name;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="subscriptionName"></param>
    /// <param name="filter">https://cloud.google.com/pubsub/docs/filtering</param>
    /// <returns></returns>
    [HttpPost("Topics/{topicName}/Subscriptions/{subscriptionName}")]
    public async Task<string> CreateSubscription(string topicName, string subscriptionName, string filter)
    {
        return (await _subscriptionManager.CreateSubscriptionWithFilter(topicName, subscriptionName, filter)).Name;
    }
}
