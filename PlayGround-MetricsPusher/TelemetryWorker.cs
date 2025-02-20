using System.Diagnostics.Metrics;

namespace PlayGround_MetricsPusher;

public class TelemetryWorker : BackgroundService
{
    private readonly ILogger<TelemetryWorker> _logger;
    private readonly IConfiguration configuration;
    private readonly Meter _meter;
    private readonly Counter<long> _metricCounter;

    public TelemetryWorker(ILogger<TelemetryWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        this.configuration = configuration;
        _meter = new Meter("PlaygroundMetricsPusher");
        _metricCounter = _meter.CreateCounter<long>("custom_metric_counter");

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string mySetting = configuration["APP_ENV"] ?? "No Enviorenment Found";

        while (!stoppingToken.IsCancellationRequested)
        {
            
            _logger.LogInformation($"{mySetting} Sending telemetry at: {DateTimeOffset.Now}");

            // Emit a metric
            _metricCounter.Add(1, new KeyValuePair<string, object?>("status", "ok"));

            await Task.Delay(1000, stoppingToken); // Run every second
        }
    }
}