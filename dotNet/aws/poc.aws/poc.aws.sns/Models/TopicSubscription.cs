#nullable disable
namespace poc.aws.sns.Models;

/// <summary>
/// Topic Subscription model
/// </summary>
public class TopicSubscription
{
    /// <summary>
    /// Gets or sets the endpoint.
    /// </summary>
    /// <value>
    /// The endpoint.
    /// </value>
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the owner.
    /// </summary>
    /// <value>
    /// The owner.
    /// </value>
    public string Owner { get; set; }

    /// <summary>
    /// Gets or sets the protocol.
    /// </summary>
    /// <value>
    /// The protocol.
    /// </value>
    public string Protocol { get; set; }

    /// <summary>
    /// Gets or sets the subscription arn.
    /// </summary>
    /// <value>
    /// The subscription arn.
    /// </value>
    public string SubscriptionArn { get; set; }

    /// <summary>
    /// Gets or sets the topic arn.
    /// </summary>
    /// <value>
    /// The topic arn.
    /// </value>
    public string TopicArn { get; set; }
}
