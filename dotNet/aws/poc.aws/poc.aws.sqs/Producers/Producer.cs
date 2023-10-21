using Amazon.SQS;
using Amazon.SQS.Model;
using poc.aws.sqs.Extensions;
using System.Text.Json;

namespace poc.aws.sqs.Producers;

public class Producer: IProducer
{
    private readonly IAmazonSQS _sQSClient;

    public Producer(IAmazonSQS sQSClient)
    {
        _sQSClient = sQSClient ?? throw new ArgumentNullException(nameof(sQSClient));
    }

    public async Task SendMessageAsync<T>(string queueUrl, T message, Dictionary<string, string>? messageAttributes = null, CancellationToken stoppingToken = default)
    {
        SendMessageRequest request = new()
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = GetMessageAttributesFromMessage(messageAttributes)
        };
        var response = await _sQSClient.SendMessageAsync(request, stoppingToken);

        response.ValidateResponse($"Message not published to {queueUrl} queue.");
    }

    private Dictionary<string, MessageAttributeValue> GetMessageAttributesFromMessage(Dictionary<string, string>? messageAttributes)
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

/// <summary>
/// SQS Message Producer
/// </summary>
public interface IProducer
{
    /// <summary>
    /// Sends the message asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queueUrl">The queue URL.</param>
    /// <param name="message">The message.</param>
    /// <param name="messageAttributes">The message attributes.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task SendMessageAsync<T>(string queueUrl, T message, Dictionary<string, string>? messageAttributes = null, CancellationToken stoppingToken = default);
}