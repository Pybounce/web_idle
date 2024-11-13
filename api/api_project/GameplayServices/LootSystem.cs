
public interface ILootSystem {

}

public class LootSystem: ILootSystem, IDisposable {

    private readonly IEventHub _eventHub;
    
    public LootSystem(IEventHub eventHub) {
        _eventHub = eventHub;
        _eventHub.Subscribe<ResourceHarvestComplete>(OnResourceHarvestComplete);
    }

    public void OnResourceHarvestComplete(ResourceHarvestComplete e) {
        // TODO: Implement loot tables so itemId can be derived from resourceNodeId 
        _eventHub.Publish<ItemGained>(new ItemGained(1, 1));
    }

    public void Dispose() {
        _eventHub.Unsubscribe<ResourceHarvestComplete>(OnResourceHarvestComplete);
    }

}

