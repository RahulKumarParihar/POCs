using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using poc.aws.core.Extensions;

namespace poc.aws.sns.Admin;

public class TopicManager: ITopicManager
{
    private readonly IAmazonSimpleNotificationService _snsClient;

    public TopicManager(IAmazonSimpleNotificationService snsClient)
    {
        _snsClient = snsClient ?? throw new ArgumentNullException(nameof(snsClient));
    }

    public async Task<string> CreateAsync(string name, string? policy = null, Dictionary<string, string>? attributes = null, Dictionary<string, string>? tags = null, CancellationToken stoppingToken = default)
    {
        CreateTopicRequest request = new()
        {
            Name = name,
            Attributes = attributes,
            Tags = GetTags(tags),
            DataProtectionPolicy = policy,  
        };

        CreateTopicResponse response = await _snsClient.CreateTopicAsync(request, stoppingToken);

        return response.TopicArn;
    }

    private static List<Tag> GetTags(Dictionary<string, string>? tags = null)
    {
        List<Tag> awsTags = new();

        if(tags is null)
            return awsTags;

        foreach(var currentTag in tags)
        {
            Tag newAwsTag = new()
            {
                Key = currentTag.Key,
                Value = currentTag.Value
            };

            awsTags.Add(newAwsTag);
        }

        return awsTags;
    }

    public async Task DeleteAsync(string topicName, CancellationToken stoppingToken = default)
    {
        string topicArn = await GetArnAsync(topicName);

        DeleteTopicRequest request = new()
        {
            TopicArn = topicArn
        };

        DeleteTopicResponse response = await _snsClient.DeleteTopicAsync(request, stoppingToken);

        response.ValidateResponse();
    }

    public async Task<string> GetArnAsync(string topicName)
    {
        Topic response = await _snsClient.FindTopicAsync(topicName);

        return response?.TopicArn!;
    }
}

/// <summary>
/// SNS topic manager
/// </summary>
public interface ITopicManager
{
    /// <summary>
    /// Creates the asynchronous.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="policy">The policy.</param>
    /// <param name="attributes">The attributes.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task<string> CreateAsync(string name, string? policy = null, Dictionary<string, string>? attributes = null, Dictionary<string, string>? tags = null, CancellationToken stoppingToken = default);

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task DeleteAsync(string topicName, CancellationToken stoppingToken = default);

    /// <summary>
    /// Gets the arn asynchronous.
    /// </summary>
    /// <param name="topicName">Name of the topic.</param>
    /// <returns></returns>
    Task<string> GetArnAsync(string topicName);
}