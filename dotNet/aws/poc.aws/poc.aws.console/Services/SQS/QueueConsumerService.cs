using poc.aws.console.Models;
using poc.aws.sqs.Admin;
using poc.aws.sqs.Consumers;
using System.Text.Json;

namespace poc.aws.console.Services.SQS;

/// <summary>
/// AWS SQS execution service
/// </summary>
/// <seealso cref="IAWSService" />
internal class QueueConsumerService : IAWSService
{
    private readonly string _queueName;
    private readonly IQueueManager _queueManager;
    private readonly IConsumer _consumer;

    public QueueConsumerService(string queueName, IQueueManager queueManager, IConsumer consumer)
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentException($"'{nameof(queueName)}' cannot be null or whitespace.", nameof(queueName));
        }

        _queueName = queueName;
        _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
        _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                string queueUrl = await _queueManager.GetQueueUrlAsync(_queueName, stoppingToken);

                var response = await _consumer.GetMessageAsync<ConsumerCreated>(queueUrl, 10, stoppingToken);

                if(response.Count > 0)
                    await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
