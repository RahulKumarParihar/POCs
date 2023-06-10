using System;
using System.Net.Http;

namespace HttpClientProject.HttpClients.GameOfThrones
{
    /// <summary>
    /// Games Of Thrones HttpClient
    /// </summary>
    public class GameOfThronesHttpClient : IGameOfThronesHttpClient
    {
        private const string baseUri = "https://www.anapioficeandfire.com/api/";

        private readonly IHttpClientFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameOfThronesHttpClient"/> class.
        /// </summary>
        /// <param name="factory"></param>
        public GameOfThronesHttpClient(IHttpClientFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<string?> GetCharactersAsync()
        {
            var client = _factory.CreateClient();

            client.BaseAddress = new Uri(baseUri);

            return await client.GetStringAsync("books");
        }
    }

    public interface IGameOfThronesHttpClient
    {
        /// <summary>
        /// Get all characters
        /// </summary>
        /// <returns></returns>
        Task<string?> GetCharactersAsync();
    }
}