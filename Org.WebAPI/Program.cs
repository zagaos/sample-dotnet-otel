using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetryTracing(budiler =>
{
    budiler
        .AddAspNetCoreInstrumentation(opt =>
        {
            opt.RecordException = true;
        })
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("Org.WebAPI")
            .AddTelemetrySdk()
        )
        .SetErrorStatusOnException(true)
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://localhost:4317"); // Signoz Endpoint
        });
});
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

builder.Services.AddOpenTelemetryTracing(budiler =>
{
    budiler
        .AddAspNetCoreInstrumentation(opt =>
        {
            opt.RecordException = true;
        })
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("Org.WebAPI")
            .AddTelemetrySdk()
        )
        .SetErrorStatusOnException(true)
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://localhost:4317"); // Signoz Endpoint
        });
});

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
