using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using PlayGround_MetricsPusher;

var builder = WebApplication.CreateBuilder(args);

string otelEndpoint = "http://otel-collector:55681"; // Uses the K8s service name

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PlaygroundMetricsPusher"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(otelEndpoint);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }))
    .WithMetrics(metricsProviderBuilder => metricsProviderBuilder
        .AddMeter("PlaygroundMetricsPusher")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(otelEndpoint);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }));

builder.Logging.AddOpenTelemetry(loggingOptions =>
{
    loggingOptions.IncludeFormattedMessage = true;
    loggingOptions.IncludeScopes = true;
    loggingOptions.ParseStateValues = true;
    loggingOptions.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri(otelEndpoint);
        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
    });
});

builder.Services.AddHostedService<TelemetryWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
