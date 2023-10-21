using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using poc.aws.sqs.Admin;
using poc.aws.sqs.Consumers;
using poc.aws.sqs.Producers;

namespace poc.aws.sqs;

/// <summary>
/// Simple queue service collections
/// </summary>
public static class SqsServiceCollections
{
    /// <summary>
    /// Adds the SQS producer.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns></returns>
    public static IServiceCollection AddSqsProducer(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSQS>()
            .AddSingleton<IProducer, Producer>()
            .TryAddSingleton<IQueueManager, QueueManager>();

        return services;
    }

    /// <summary>
    /// Adds the SQS consumer.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns></returns>
    public static IServiceCollection AddSqsConsumer(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSQS>()
            .AddSingleton<IConsumer, Consumer>()
            .TryAddSingleton<IQueueManager, QueueManager>();

        return services;
    }
}
