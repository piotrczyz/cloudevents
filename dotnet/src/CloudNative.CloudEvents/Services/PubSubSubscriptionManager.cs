using Google.Cloud.PubSub.V1;
using Grpc.Core;

namespace CloudEvents.Services;

public class PubSubSubscriptionManager
{
    private readonly PubSubOptions _config;

    public PubSubSubscriptionManager(PubSubOptions config)
    {   
        _config = config;
    }
    
    public async Task<Topic> CreateTopic(string topicId)
    {
        var publisher = await PublisherServiceApiClient.CreateAsync();
        var topicName = TopicName.FromProjectTopic(_config.ProjectId, topicId);
        var topic = new Topic
        {
            Name = topicName.ToString()
        };

        Topic? receivedTopic = null;
        try
        {
            receivedTopic = await publisher.CreateTopicAsync(topic);
            Console.WriteLine($"Topic {topic.Name} created.");
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"Topic {topicName} already exists.");
            receivedTopic = await publisher.GetTopicAsync(topicName);
        }
        return receivedTopic;
    }

    public async Task<Subscription> CreateSubscriptionWithFilter(string topicId, string subscriptionId, string filter)
    {
        var subscriber = await SubscriberServiceApiClient.CreateAsync();
        var topicName = TopicName.FromProjectTopic(_config.ProjectId, topicId);
        var subscriptionName = SubscriptionName.FromProjectSubscription(_config.ProjectId, subscriptionId);
        Subscription subscription;

        var subscriptionRequest = new Subscription
        {
            SubscriptionName = subscriptionName,
            TopicAsTopicName = topicName,
            Filter = filter
        };

        try
        {
            subscription = await subscriber.CreateSubscriptionAsync(subscriptionRequest);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
            subscription = await subscriber.GetSubscriptionAsync(subscriptionName);
        }
        return subscription;
    }
}