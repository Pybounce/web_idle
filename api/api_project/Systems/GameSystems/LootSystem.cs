
public interface ILootSystem {

}

public class LootSystem: ILootSystem, IDisposable {

    private readonly IEventHub _eventHub;
    private readonly ILootDataService _lootDataService;
    private readonly IRandomNumberGenerator _rng;
    
    public LootSystem(IEventHub eventHub, ILootDataService lootDataService, IRandomNumberGenerator rng) {
        _eventHub = eventHub;
        _eventHub.Subscribe<ResourceHarvestCompleteEvent>(OnResourceHarvestComplete);
        _lootDataService = lootDataService;
        _rng = rng;
    }

    public void OnResourceHarvestComplete(ResourceHarvestCompleteEvent e) {
        if (_lootDataService.TryGetTable(e.ResourceId, out LootTableDocument table)) {
            var itemsGained = CalcItemLoot(table);
            foreach (var (itemId, amount) in itemsGained) {
                _eventHub.Publish(new ItemGainedEvent(itemId, amount));
            }
        }
        else {
            // log warning?
        }
    }

    public void Dispose() {
        _eventHub.Unsubscribe<ResourceHarvestCompleteEvent>(OnResourceHarvestComplete);
    }

    private List<(int itemId, int amount)> CalcItemLoot(LootTableDocument table) {
        var output = new List<(int, int)>();

        foreach (var itemChance in table.ItemChances) {
            if (_rng.Next(itemChance.ChanceDenominator) == 0) {
                output.Add((itemChance.ItemId, 1));
            }
        }
        return output;
    }

}

