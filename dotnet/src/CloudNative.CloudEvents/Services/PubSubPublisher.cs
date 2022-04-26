using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using DotnetApplication;
using Google.Cloud.PubSub.V1;
using Encoding = System.Text.Encoding;

namespace CloudEvents.Services;

using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PubSubPublisher
{
    private static readonly CloudEventFormatter Formatter = new JsonEventFormatter();
    private readonly PubSubOptions _config;

    public PubSubPublisher(PubSubOptions config)
    {   
        _config = config;
    }
    
    public async Task<int> PublishMessagesAsync(IEnumerable<CloudEvent> cloudEvents)
    {
        TopicName topicName = TopicName.FromProjectTopic(_config.ProjectId, _config.TopicId);
        PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

        int publishedMessageCount = 0;
        var publishTasks = cloudEvents.Select(async cloudEvent =>
        {
            try
            {
                var bytes = Formatter.EncodeStructuredModeMessage(cloudEvent, out var contentTyp);
                var json = Encoding.UTF8.GetString(bytes.Span);
                var messageId = await publisher.PublishAsync(json);
                
                Console.WriteLine($"Published message {messageId}");
                Interlocked.Increment(ref publishedMessageCount);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred when publishing message {cloudEvent}: {exception.Message}");
            }
        });
        await Task.WhenAll(publishTasks);
        return publishedMessageCount;
    }
    
    public async Task<int> PullMessagesAsync(bool acknowledge, string subscriptionId)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_config.ProjectId, subscriptionId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        // SubscriberClient runs your message handle function on multiple
        // threads to maximize throughput.
        int messageCount = 0;
        Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
        {
            var parser = HelloWorldDotNetMessage.Parser;
            var parsed = parser.ParseFrom(message.ToByteArray());
            Console.WriteLine($"Message {message.MessageId}: {parsed.Message}");
            Interlocked.Increment(ref messageCount);
            return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
        });
        // Run for 5 seconds.
        await Task.Delay(5000);
        await subscriber.StopAsync(CancellationToken.None);
        // Lets make sure that the start task finished successfully after the call to stop.
        await startTask;
        return messageCount;
    }
}