using Amazon.Runtime;

namespace poc.aws.sqs.Extensions;

/// <summary>
/// Extensions for Aws web service
/// </summary>
internal static class AWSWebServiceExtensions
{
    /// <summary>
    /// Validates the response.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ValidateResponse(this AmazonWebServiceResponse response, string? message = null)
    {
        if (response is null || response.HttpStatusCode > System.Net.HttpStatusCode.OK)
            throw new InvalidOperationException(message ?? "Invalid Operation");
    }
}
