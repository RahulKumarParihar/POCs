using Microsoft.Extensions.DependencyInjection;
using poc.aws.sqs.Admin;
using poc.aws.sqs.Producers;

namespace poc.aws.console.Services;

/// <summary>
/// Type of aws service
/// </summary>
internal enum AWSServiceType
{
    /// <summary>
    /// The SQS
    /// </summary>
    sqs
}

/// <summary>
/// AWS Service Factory
/// </summary>
internal class AWSServiceFactory
{
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
            AWSServiceType.sqs => 
                new SqsProducerService("demo_queue", _serviceProvider.GetRequiredService<IQueueManager>(), _serviceProvider.GetRequiredService<IProducer>()),
            _ => throw new KeyNotFoundException(nameof(serviceType)),
        };
    }
}
