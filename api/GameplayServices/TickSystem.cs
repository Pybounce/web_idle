



public class TickSystem : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer = null;
    public event Action OnTick;

    public TickSystem(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(MoveUnits, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void MoveUnits(object? state) {
        using (IServiceScope scope = _serviceProvider.CreateScope()) {
            IScopedTickSystem scopedTickSystem = scope.ServiceProvider.GetRequiredService<IScopedTickSystem>();
            scopedTickSystem.Tick();
        }
        OnTick?.Invoke();
    }

    public void Dispose() {
        _timer?.Dispose();
    }

}

