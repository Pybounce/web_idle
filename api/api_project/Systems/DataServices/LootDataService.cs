

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

public interface ILootDataService: IDataService {
    public bool TryGetTable(int resourceId, out LootTable table);
}

public class LootDataService: ILootDataService, IDisposable {

    private readonly ICosmosClientFactory _cosmosClientFactory;
    private LootTable[] _lootTables;
    
    public LootDataService(ICosmosClientFactory cosmosClientFactory) {
        _cosmosClientFactory = cosmosClientFactory;
        _lootTables = [];
    }

    public async Task InitAsync() {
        Console.WriteLine("initing");
        var dbClient = _cosmosClientFactory.NewClient();
        await LoadData(dbClient);
    }

    public bool TryGetTable(int resourceId, out LootTable table) {
        foreach (var lt in _lootTables) {
            if (lt.ResourceId == resourceId && lt.ItemChances != null && lt.ItemChances.Length > 0) {
                table = lt;
                return true;
            }
        }
        table = null;
        return false;
    }

    private async Task LoadData(CosmosClient client) {
        var containerClient = client.GetContainer("main-db", "loot-tables");
        var feedItr = containerClient.GetItemLinqQueryable<LootTable>().ToFeedIterator();
        var results = new List<LootTable>();
        while (feedItr.HasMoreResults) {
            var response = await feedItr.ReadNextAsync();
            results.AddRange(response.Resource);
        }
        _lootTables = results.ToArray();
    }

    public void Dispose() {
        Console.WriteLine("disposing lootdataservice");
    }
}

