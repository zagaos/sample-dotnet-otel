using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging => logging.AddOpenTelemetry(openTelemetryLoggerOptions =>
{
    openTelemetryLoggerOptions.SetResourceBuilder(
        ResourceBuilder.CreateEmpty()
            .AddService("Org.WebAPI")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = "development"
            }));
        
    openTelemetryLoggerOptions.IncludeScopes = true;
    openTelemetryLoggerOptions.IncludeFormattedMessage = true;
    
    openTelemetryLoggerOptions.AddOtlpExporter(exporter =>
    {
       // exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
        //exporter.Endpoint = new Uri("https://seq.example.com/ingest/otlp/v1/logs");
        //exporter.Headers = "X-Seq-ApiKey=fd8sHZ28fajva8t32Ngjfdisp";
        exporter.Protocol = OtlpExportProtocol.Grpc;
            // option.Endpoint = new Uri("https://otel-custom-grpc-otel.apps.cluster-hvnhl.hvnhl.sandbox2235.opentlc.com"); // Expose gRPC endpoint as route doesn't work!!!
            exporter.Endpoint = new Uri("http://otel-collector.observability.svc.cluster.local:4317"); // Only gRPC service endpoint works, 
            exporter.ExportProcessorType = ExportProcessorType.Batch;
    });
}));

// builder.Services.AddOpenTelemetryTracing(budiler =>
// {
//     budiler
//         .AddAspNetCoreInstrumentation(opt =>
//         {
//             opt.RecordException = true;
//         })
//         .SetResourceBuilder(ResourceBuilder.CreateDefault()
//             .AddService("Org.WebAPI")
//             .AddTelemetrySdk()
//         )
//         .SetErrorStatusOnException(true)
//         .AddOtlpExporter(options =>
//         {
//             options.Endpoint = new Uri("http://otel-collector.observability.svc.cluster.local:4317");
//         });
// });

// builder.Logging.ClearProviders();
// builder.Logging.AddOpenTelemetry(options =>
// {
//     options
//         .SetResourceBuilder(ResourceBuilder.CreateDefault()
//             .AddService("Org.WebAPI")
//             .AddEnvironmentVariableDetector()
//             .AddTelemetrySdk()
//         )
//         // .SetErrorStatusOnException()
//         .AddConsoleExporter()
//         .AddOtlpExporter(opt =>
//         {
//             opt.Endpoint = new Uri("http://localhost:4317"); // Signoz Endpoint
//         });
// });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
