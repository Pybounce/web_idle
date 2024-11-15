
using Microsoft.Azure.Cosmos;

public interface IDbIO {
    public Task SavePlayerInventoryAsync(Inventory inventory);
}

public class DbIO: IDbIO {
    
    private readonly CosmosClient _dbClient;

    public DbIO(ICosmosClientFactory cosmosClientFactory) {
        _dbClient = cosmosClientFactory.NewClient();
    }

    public async Task SavePlayerInventoryAsync(Inventory inventory) {
        var userId = "0";
        var inventoryId = "0";
        var playerInventory = new PlayerInventory() {
            UserId = userId,
            id = inventoryId,
            Items = inventory.GetItems()
        };

        var container = _dbClient.GetDatabase("main-db").GetContainer("main-container");

        await container.UpsertItemAsync<PlayerInventory>(playerInventory);

    }

    public void Dispose() {
        _dbClient.Dispose();
    }

}