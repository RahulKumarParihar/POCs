using Amazon.SQS;
using Amazon.SQS.Model;
using poc.aws.core.Extensions;

namespace poc.aws.sqs.Admin;

public class QueueManager: IQueueManager
{
    private readonly IAmazonSQS _sQSClient;

    public QueueManager(IAmazonSQS sQSClient)
    {
        _sQSClient = sQSClient ?? throw new ArgumentNullException(nameof(sQSClient));
    }

    public async Task<string> CreateQueueAsync(string queueName, Dictionary<string, string>? attributes = null, Dictionary<string, string>? tags = null, CancellationToken stoppingToken = default)
    {
        CreateQueueRequest request = new()
        {
            QueueName = queueName,
            Attributes = attributes,
            Tags = tags
        };

        var response = await _sQSClient.CreateQueueAsync(request);

        response.ValidateResponse($"{queueName} queue creation failed.");

        return response.QueueUrl;
    }

    public async Task DeleteQueueAsync(string queueName, CancellationToken stoppingToken = default)
    {
        var queueUrl = await GetQueueUrlAsync(queueName, stoppingToken);

        var response = await _sQSClient.DeleteQueueAsync(queueUrl, stoppingToken);

        response.ValidateResponse($"Failed to delete {queueName} queue.");
    }

    public async Task<string> GetQueueUrlAsync(string queueName, CancellationToken stoppingToken = default)
    {
        var response = await _sQSClient.GetQueueUrlAsync(queueName, stoppingToken);

        response.ValidateResponse($"Unable to fetch queue url for {queueName} queue.");

        return response.QueueUrl;
    }
}

/// <summary>
/// Manages the queue
/// </summary>
public interface IQueueManager
{
    /// <summary>
    /// Creates the queue asynchronous.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="attributes">The attributes.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task<string> CreateQueueAsync(string queueName, Dictionary<string, string>? attributes = null, Dictionary<string, string>? tags = null, CancellationToken stoppingToken = default);

    /// <summary>
    /// Deletes the queue asynchronous.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task DeleteQueueAsync(string queueName, CancellationToken stoppingToken = default);

    /// <summary>
    /// Gets the queue URL asynchronous.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task<string> GetQueueUrlAsync(string queueName, CancellationToken stoppingToken = default);
}