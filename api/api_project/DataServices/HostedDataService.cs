

public class HostedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public HostedDataService(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope()) {
            var lootDataService = scope.ServiceProvider.GetRequiredService<ILootDataService>();
            await lootDataService.InitAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}