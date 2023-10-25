using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using poc.aws.core.Extensions;
using poc.aws.sns.Admin;
using System.Text.Json;

namespace poc.aws.sqs.Producers;

public class TopicMessageProducer: ITopicMessageProducer
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly ITopicManager _topicManager;

    public TopicMessageProducer(IAmazonSimpleNotificationService snsClient, ITopicManager topicManager)
    {
        _snsClient = snsClient ?? throw new ArgumentNullException(nameof(snsClient));
        _topicManager = topicManager ?? throw new ArgumentNullException(nameof(topicManager));
    }

    public async Task PublishAsync<T>(string topicName, T message, Dictionary<string, string>? messageAttributes = null, CancellationToken stoppingToken = default)
    {
        var topicArn = await _topicManager.GetArnAsync(topicName);

        PublishRequest request = new()
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message),
            MessageAttributes = GetMessageAttributesFromMessage(messageAttributes)
        };

        PublishResponse response = await _snsClient.PublishAsync(request, stoppingToken);

        response.ValidateResponse();
    }

    private static Dictionary<string, MessageAttributeValue> GetMessageAttributesFromMessage(Dictionary<string, string>? messageAttributes)
    {
        Dictionary<string, MessageAttributeValue> attributes = new();

        if (messageAttributes is null)
            return attributes;

        foreach (var currentAttributes in messageAttributes)
        {
            if (string.IsNullOrWhiteSpace(currentAttributes.Key) || string.IsNullOrWhiteSpace(currentAttributes.Value))
                continue;

            attributes.Add(currentAttributes.Key, new MessageAttributeValue() { DataType = "String", StringValue = currentAttributes.Value });
        }

        return attributes;
    }
}

public interface ITopicMessageProducer
{
    /// <summary>
    /// Publishes the asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="message">The message.</param>
    /// <param name="messageAttributes">The message attributes.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task PublishAsync<T>(string topicName, T message, Dictionary<string, string>? messageAttributes = null, CancellationToken stoppingToken = default);
}