
using Microsoft.Azure.Cosmos;

public interface IDbIO {

}

public class DbIO: IDbIO {
    
    private readonly CosmosClient _dbClient;
    private readonly IScopedTickSystem _tickSystem;

    public DbIO(ICosmosClientFactory cosmosClientFactory, IScopedTickSystem tickSystem) {
        _dbClient = cosmosClientFactory.NewClient();
        _tickSystem = tickSystem;
        _tickSystem.OnTick += OnTick;

    }

    private int x = -1;
    private async void OnTick() {
        x += 1;
        if (x % 10 == 0) {
            var container = _dbClient.GetDatabase("main-db").GetContainer("main-container");
            var pi = new PlayerInventory() {
                Something = x * 100,
                UserId = x.ToString(),
                id = x.ToString()
            };
            Console.WriteLine("writing");
            await container.CreateItemAsync<PlayerInventory>(pi, new PartitionKey(pi.UserId));
            Console.WriteLine("written");
        }
    }

    public void Dispose() {
        _tickSystem.OnTick -= OnTick;
        _dbClient.Dispose();
    }

}