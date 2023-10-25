using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using poc.aws.core.Extensions;
using poc.aws.sns.Models;
using poc.aws.sqs.Admin;

namespace poc.aws.sns.Admin;

public class TopicSubscriptionManager: ITopicSubscriptionManager
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly IAmazonSQS _sqsClient;
    private readonly ITopicManager _topicManager;
    private readonly IQueueManager _queueManager;

    public TopicSubscriptionManager(IAmazonSimpleNotificationService snsClient, IAmazonSQS sqsClient, ITopicManager topicManager, IQueueManager queueManager)
    {
        _snsClient = snsClient ?? throw new ArgumentNullException(nameof(snsClient));
        _sqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
        _topicManager = topicManager ?? throw new ArgumentNullException(nameof(topicManager));
        _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
    }

    public async Task<List<TopicSubscription>> GetSubscriptionsByTopicAsync(string topicName, CancellationToken stoppingToken = default)
    {
        var topicArn = await _topicManager.GetArnAsync(topicName);

        string nextToken = null!;
        ListSubscriptionsByTopicResponse response;
        List<TopicSubscription> topicSubscriptions = new();

        do
        {
            response = await _snsClient.ListSubscriptionsByTopicAsync(topicArn, nextToken, stoppingToken);

            response.ValidateResponse();

            foreach (var sub in response.Subscriptions)
            {
                topicSubscriptions.Add(new TopicSubscription()
                {
                    Endpoint = sub.Endpoint,
                    Owner = sub.Owner,
                    Protocol = sub.Protocol,
                    SubscriptionArn = sub.SubscriptionArn,
                    TopicArn = sub.TopicArn
                });
            }

            nextToken = response.NextToken;
        } while (nextToken is not null);

        return topicSubscriptions;
    }

    public async Task SubscribeQueueToTopicAsync(string topicName, string queueName, CancellationToken stoppingToken = default)
    {
        var topicArn = await _topicManager.GetArnAsync(topicName);

        var queueUrl = await _queueManager.GetQueueUrlAsync(queueName, stoppingToken);

        await _snsClient.SubscribeQueueAsync(topicArn, _sqsClient, queueUrl);
    }

    public async Task SubscribeQueueToManyTopicsAsync(List<string> topicNames, string queueName, CancellationToken stoppingToken = default)
    {
        List<string> topicArns = new();

        foreach(var name in topicNames)
        {
            topicArns.Add(await _topicManager.GetArnAsync(name));
        }

        var queueUrl = await _queueManager.GetQueueUrlAsync(queueName, stoppingToken);

        await _snsClient.SubscribeQueueToTopicsAsync(topicArns, _sqsClient, queueUrl);
    }

    public async Task UnsubscribeAllSubscriptionsFromTopic(string topicName, CancellationToken stoppingToken = default)
    {
        List<TopicSubscription> subscriptions = await GetSubscriptionsByTopicAsync(topicName, stoppingToken);

        foreach(var sub in subscriptions)
        {
            var response = await _snsClient.UnsubscribeAsync(sub.SubscriptionArn, stoppingToken);

            response.ValidateResponse();
        }
    }

    public async Task UnsubscribeSubscription(string subscriptionArn, CancellationToken stoppingToken = default)
    {
        var response = await _snsClient.UnsubscribeAsync(subscriptionArn, stoppingToken);

        response.ValidateResponse();
    }
}

/// <summary>
/// Topic subscription manager
/// </summary>
public interface ITopicSubscriptionManager
{
    /// <summary>
    /// Gets the subscriptions by topic asynchronous.
    /// </summary>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<List<TopicSubscription>> GetSubscriptionsByTopicAsync(string topicName, CancellationToken stoppingToken = default);

    /// <summary>
    /// Subscribes the queue to many topics asynchronous.
    /// </summary>
    /// <param name="topicNames">The topic names.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task SubscribeQueueToManyTopicsAsync(List<string> topicNames, string queueName, CancellationToken stoppingToken = default);

    /// <summary>
    /// Subscribes the queue to topic asynchronous.
    /// </summary>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task SubscribeQueueToTopicAsync(string topicName, string queueName, CancellationToken stoppingToken = default);

    /// <summary>
    /// Unsubscribes all subscriptions from topic.
    /// </summary>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task UnsubscribeAllSubscriptionsFromTopic(string topicName, CancellationToken stoppingToken = default);

    /// <summary>
    /// Unsubscribes the subscription.
    /// </summary>
    /// <param name="subscriptionArn">The subscription arn.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task UnsubscribeSubscription(string subscriptionArn, CancellationToken stoppingToken = default);
}