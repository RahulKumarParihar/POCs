using HttpClientProject.HttpClients.GameOfThrones;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HttpClientProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var gameOfThronesHttpClient = host.Services.GetRequiredService<IGameOfThronesHttpClient>();
            await gameOfThronesHttpClient.GetCharactersAsync();
            //Console.WriteLine(response);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<IGameOfThronesHttpClient, GameOfThronesHttpClient>();
                });
    }
}