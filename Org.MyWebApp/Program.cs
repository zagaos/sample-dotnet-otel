using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddOpenTelemetryTracing(budiler =>
{
    budiler
        .AddAspNetCoreInstrumentation()
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("MyWebApp")
            .AddTelemetrySdk()
        )
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://localhost:4317"); // Signoz Endpoint
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
