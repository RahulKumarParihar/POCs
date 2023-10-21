using poc.aws.console.Models;
using poc.aws.sqs.Admin;
using poc.aws.sqs.Producers;

namespace poc.aws.console.Services.SQS;

/// <summary>
/// AWS SQS execution service
/// </summary>
/// <seealso cref="IAWSService" />
internal class SqsProducerService : IAWSService
{
    private readonly string _queueName;
    private readonly IQueueManager _queueManager;
    private readonly IProducer _producer;

    public SqsProducerService(string queueName, IQueueManager queueManager, IProducer producer)
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentException($"'{nameof(queueName)}' cannot be null or whitespace.", nameof(queueName));
        }

        _queueName = queueName;
        _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                string queueUrl;
                try
                {
                    queueUrl = await _queueManager.GetQueueUrlAsync(_queueName, stoppingToken);
                }
                catch (Exception)
                {
                    queueUrl = await _queueManager.CreateQueueAsync(_queueName, stoppingToken: stoppingToken);
                }

                ConsumerCreated consumer = new()
                {
                    Id = Guid.NewGuid(),
                    FullName = "FirstName LastName",
                    Email = "FirstName@Test.com",
                    GitHubUsername = "admin",
                    DateOfBirth = new DateTime(1990, 12, 13)
                };

                await _producer.SendMessageAsync(queueUrl,
                    consumer,
                    new() { { "MessageType", nameof(ConsumerCreated) } },
                    stoppingToken);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
