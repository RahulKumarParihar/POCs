using Amazon.SQS;
using Amazon.SQS.Model;
using poc.aws.sqs.Extensions;

namespace poc.aws.sqs.Producers;

public class Producer: IProducer
{
    private readonly IAmazonSQS _sQSClient;

    public Producer(IAmazonSQS sQSClient)
    {
        _sQSClient = sQSClient ?? throw new ArgumentNullException(nameof(sQSClient));
    }

    public async Task SendMessageAsync(string queueUrl, string messageBody, Dictionary<string, string>? messageAttributes = null, CancellationToken stoppingToken = default)
    {
        SendMessageRequest request = new()
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody,
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

            attributes.Add(currentAttributes.Key, new MessageAttributeValue() { DataType = "string", StringValue = currentAttributes.Value });
        }

        return attributes;
    }
}

public interface IProducer
{
    /// <summary>
    /// Sends the message asynchronous.
    /// </summary>
    /// <param name="queueUrl">The queue URL.</param>
    /// <param name="messageBody">The message body.</param>
    /// <param name="messageAttributes">The message attributes.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task SendMessageAsync(string queueUrl, string messageBody, Dictionary<string, string>? messageAttributes = null, CancellationToken stoppingToken = default);
}