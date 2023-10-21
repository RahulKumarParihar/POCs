namespace poc.aws.console.Services;

/// <summary>
/// AWS Service 
/// </summary>
internal interface IAWSService
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken stoppingToken);
}
