using System.Diagnostics;
using System.Diagnostics.Metrics;
namespace Org.Worker;

public class Worker : IHostedService, IDisposable
{
    private readonly ILogger<Worker> _logger;
    private Timer? _timer = null;

    private static Counter<int> _noOfRunsCounter;
    private static Histogram<float> _timeTakenHistogram;
    private static readonly Meter _baseMeter = new("WorkerMeter", "22.05");

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _noOfRunsCounter = _baseMeter.CreateCounter<int>("NoOfRuns");
        _timeTakenHistogram = _baseMeter.CreateHistogram<float>("TimeTaken", "ms");
        _baseMeter.CreateObservableGauge("ThreadCount", () => ThreadPool.ThreadCount);
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        // Basic Counter
        _noOfRunsCounter.Add(1);

        // Sleeping for random seconds
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        var stopwatch = Stopwatch.StartNew();
        var randomValue = new Random().Next(2, 10);
        Thread.Sleep(TimeSpan.FromSeconds(randomValue));
        stopwatch.Stop();

        // Recording Historgram Counter
        _timeTakenHistogram.Record(stopwatch.ElapsedMilliseconds);

        _logger.LogInformation($"Timed Hosted Service sleeping for {randomValue} seconds");
        _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}