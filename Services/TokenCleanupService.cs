using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleApi.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(ILogger<TokenCleanupService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TokenCleanupService başladı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Background job çalıştı: {time}", DateTime.UtcNow);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("TokenCleanupService durdu.");
    }
}