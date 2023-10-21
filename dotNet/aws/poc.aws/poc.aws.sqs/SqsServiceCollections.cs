using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using poc.aws.sqs.Admin;
using poc.aws.sqs.Producers;

namespace poc.aws.sqs;

public static class SqsServiceCollections
{
    public static IServiceCollection AddProducer(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSQS>()
            .AddSingleton<IQueueManager, QueueManager>()
            .AddSingleton<IProducer, Producer>();

        return services;
    }
}
