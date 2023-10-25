using poc.aws.console.Models;
using poc.aws.sns.Admin;
using poc.aws.sqs.Producers;

namespace poc.aws.console.Services.SNS;

/// <summary>
/// AWS SQS execution service
/// </summary>
/// <seealso cref="IAWSService" />
internal class TopicProducerService : IAWSService
{
    private readonly string _topicName;
    private readonly string _queueName;
    private readonly ITopicManager _topicManager;
    private readonly ITopicSubscriptionManager _subscriptionManager;
    private readonly ITopicMessageProducer _producer;

    public TopicProducerService(string topicName, string queueName, ITopicManager topicManager, ITopicSubscriptionManager subscriptionManager, ITopicMessageProducer producer)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            throw new ArgumentException($"'{nameof(topicName)}' cannot be null or whitespace.", nameof(topicName));
        }

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentException($"'{nameof(queueName)}' cannot be null or whitespace.", nameof(queueName));
        }

        _topicName = topicName;
        _queueName = queueName;
        _topicManager = topicManager ?? throw new ArgumentNullException(nameof(topicManager));
        _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string topicArn = await _topicManager.GetArnAsync(_topicName);

        if (string.IsNullOrWhiteSpace(topicArn))
        {
            topicArn = await _topicManager.CreateAsync(_topicName, stoppingToken: stoppingToken);
        }

        var subscriptions = await _subscriptionManager.GetSubscriptionsByTopicAsync(_topicName);

        if (!subscriptions.Any())
            await _subscriptionManager.SubscribeQueueToTopicAsync(_topicName, _queueName, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ConsumerCreated consumer = new()
                {
                    Id = Guid.NewGuid(),
                    FullName = "SnsFirstName SnsLastName",
                    Email = "sns.FirstName@Test.com",
                    GitHubUsername = "sns_admin",
                    DateOfBirth = new DateTime(1990, 12, 13)
                };

                await _producer.PublishAsync(_topicName,
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
