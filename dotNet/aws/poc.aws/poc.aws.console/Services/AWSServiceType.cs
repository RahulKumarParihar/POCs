namespace poc.aws.console.Services;

/// <summary>
/// Type of aws service
/// </summary>
internal enum AWSServiceType
{
    /// <summary>
    /// The SQS producer
    /// </summary>
    sqsProducer,

    /// <summary>
    /// The SQS consumer
    /// </summary>
    sqsConsumer,

    /// <summary>
    /// The SNS
    /// </summary>
    sns,
}
