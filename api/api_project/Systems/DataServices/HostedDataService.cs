

public class HostedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public HostedDataService(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope()) {
            var dataServices = new List<IDataService>() {
                scope.ServiceProvider.GetRequiredService<ILootDataService>()
            };
            foreach (var dataService in dataServices) {
                await dataService.InitAsync();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public interface IDataService {
    public Task InitAsync();
}