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
                    .AddSqsProducer();
            });

        IHost host = hostBuilder.Build();

        try
        {
            var awsServiceFactory = host.Services.GetRequiredService<AWSServiceFactory>();

            var awsService = awsServiceFactory.GetAWSService(AWSServiceType.sqs);
            CancellationTokenSource producerCancellationTokenSource = new();
            producerCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            await awsService.ExecuteAsync(producerCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
    }
}