using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using poc.aws.console.Services;
using poc.aws.sqs;

namespace poc.aws.console;

internal class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services
                    .AddSingleton<AWSServiceFactory>()
                    .AddSqsProducer()
                    .AddSqsConsumer();
            });

        IHost host = hostBuilder.Build();

        var awsServiceFactory = host.Services.GetRequiredService<AWSServiceFactory>();

        await ExecuteSQSAsync(awsServiceFactory);
    }

    /// <summary>
    /// Executes the SQS asynchronous.
    /// </summary>
    /// <param name="awsServiceFactory">The aws service factory.</param>
    static async Task ExecuteSQSAsync(AWSServiceFactory awsServiceFactory)
    {
        try
        {
            // SQS Producer
            var sqsProducerService = awsServiceFactory.GetAWSService(AWSServiceType.sqsProducer);
            CancellationTokenSource producerCancellationTokenSource = new();
            producerCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            await sqsProducerService.ExecuteAsync(producerCancellationTokenSource.Token);

            // SQS Consumer
            var sqsConsumer = awsServiceFactory.GetAWSService(AWSServiceType.sqsConsumer);
            CancellationTokenSource consumerCancellationTokenSource = new();
            consumerCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            await sqsConsumer.ExecuteAsync(consumerCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
    }
}