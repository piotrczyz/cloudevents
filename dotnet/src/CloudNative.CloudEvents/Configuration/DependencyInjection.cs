using CloudEvents.Services;

namespace CloudEvents.Configuration;

public static class DependencyInjection
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("PubSub").Get<PubSubOptions>());
        services.AddSingleton<PubSubPublisher>();
        services.AddSingleton<PubSubSubscriptionManager>();
    }
}    
