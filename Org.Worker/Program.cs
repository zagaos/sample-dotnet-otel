using Org.Worker;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddOpenTelemetryMetrics(options =>
        {
            options
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("Worker")
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk()
                )
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("http://localhost:4317");
                });
        });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
