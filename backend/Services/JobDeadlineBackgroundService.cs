using backend.Options;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class JobDeadlineBackgroundService(
    IServiceProvider serviceProvider,
    IOptions<JobDeadlineOptions> options,
    ILogger<JobDeadlineBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(1, options.Value.CheckIntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var jobStatusService = scope.ServiceProvider.GetRequiredService<JobStatusService>();
                var (delayed, passed) = await jobStatusService.ProcessOverdueJobsAsync();

                if (delayed > 0)
                    logger.LogInformation("Marked {Count} overdue job(s) as delayed.", delayed);
                if (passed > 0)
                    logger.LogInformation("Marked {Count} expired unfilled job(s) as passed.", passed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing overdue jobs.");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}
