using Microsoft.Extensions.DependencyInjection;
using poc.aws.console.Services.SNS;
using poc.aws.console.Services.SQS;
using poc.aws.sns.Admin;
using poc.aws.sqs.Admin;
using poc.aws.sqs.Consumers;
using poc.aws.sqs.Producers;

namespace poc.aws.console.Services;

/// <summary>
/// AWS Service Factory
/// </summary>
internal class AWSServiceFactory
{
    private const string QUEUE_NAME = "demo_queue";
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AWSServiceFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="ArgumentNullException">serviceProvider</exception>
    public AWSServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Gets the aws service.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns></returns>
    public IAWSService GetAWSService(AWSServiceType serviceType)
    {
        return serviceType switch
        {
            AWSServiceType.sqsProducer => 
                new QueueProducerService(QUEUE_NAME, _serviceProvider.GetRequiredService<IQueueManager>(), _serviceProvider.GetRequiredService<IProducer>()),
            AWSServiceType.sqsConsumer =>
                new QueueConsumerService(QUEUE_NAME, _serviceProvider.GetRequiredService<IQueueManager>(), _serviceProvider.GetRequiredService<IConsumer>()),            
            AWSServiceType.sns =>
                new TopicProducerService("demo_sns", QUEUE_NAME, _serviceProvider.GetRequiredService<ITopicManager>(), _serviceProvider.GetRequiredService<ITopicSubscriptionManager>(), _serviceProvider.GetRequiredService<ITopicMessageProducer>()),
            _ => throw new KeyNotFoundException(nameof(serviceType)),
        };
    }
}
