using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using poc.aws.sns.Admin;
using poc.aws.sqs.Producers;

namespace poc.aws.sqs;

/// <summary>
/// Simple queue service collections
/// </summary>
public static class SnsServiceCollections
{
    /// <summary>
    /// Adds the SNS managers.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns></returns>
    public static IServiceCollection AddSnsManagers(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>()
            .AddSqsManagers()
            .TryAddSingleton<ITopicManager, TopicManager>();

        services.TryAddSingleton<ITopicSubscriptionManager, TopicSubscriptionManager>();

        return services;
    }

    /// <summary>
    /// Adds the SNS producer.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns></returns>
    public static IServiceCollection AddSnsProducer(this IServiceCollection services) 
        => services.AddSnsManagers()
            .AddSingleton<ITopicMessageProducer, TopicMessageProducer>();
}
