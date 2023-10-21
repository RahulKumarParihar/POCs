using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using poc.aws.console.Services;
using poc.aws.sqs;

namespace poc.aws.console;

internal class Program
{
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

        try
        {
            var awsServiceFactory = host.Services.GetRequiredService<AWSServiceFactory>();

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