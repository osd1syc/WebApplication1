
using Microsoft.Data.SqlClient;

namespace WebApplication1.Data
{
    public class LogCleanupService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LogCleanupService> _logger;

        public LogCleanupService(IConfiguration configuration, ILogger<LogCleanupService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LogCleanupService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("LogCleanupService is working.");
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                DeleteOldLogs();
            }
        }

        private void DeleteOldLogs()
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("EFCoreDBConnection")))
                {
                    var command = new SqlCommand("DELETE FROM Logs WHERE TimeStamp < DATEADD(day, -30, GETDATE())", connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    int affectedRows = command.ExecuteNonQuery();
                    _logger.LogInformation($"{affectedRows} log entries deleted.");
                }

                _logger.LogInformation("Logs cleaned up successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Cleaning Logs.");
            }
        }
    }
}
