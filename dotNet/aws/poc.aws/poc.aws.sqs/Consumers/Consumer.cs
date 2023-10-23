using Amazon.SQS;
using Amazon.SQS.Model;
using poc.aws.core.Extensions;
using System.Text.Json;

namespace poc.aws.sqs.Consumers
{
    public class Consumer: IConsumer
    {
        private readonly IAmazonSQS _sQSClient;

        public Consumer(IAmazonSQS sQSClient)
        {
            _sQSClient = sQSClient ?? throw new ArgumentNullException(nameof(sQSClient));
        }

        public async Task<List<T>> GetMessageAsync<T>(string queueUrl, int maxMessages, CancellationToken stoppingToken) where T : class
        {
            ReceiveMessageRequest request = new()
            {
                QueueUrl = queueUrl,
                AttributeNames = new List<string>() { "All" },
                MessageAttributeNames = new List<string>() { "All" },
                MaxNumberOfMessages = maxMessages
            };
            
            var response = await _sQSClient.ReceiveMessageAsync(request, stoppingToken);

            response.ValidateResponse();

            List<T> result = new();

            foreach (var message in response.Messages)
            {
                try
                {
                    result.Add(JsonSerializer.Deserialize<T>(message.Body)!);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// SQS Consumer
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// Gets the message asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueUrl">The queue URL.</param>
        /// <param name="maxMessages">The maximum messages.</param>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns></returns>
        Task<List<T>> GetMessageAsync<T>(string queueUrl, int maxMessages, CancellationToken stoppingToken) where T : class;
    }
}
