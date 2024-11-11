
public interface IXpSystem {

}

public class XpSystem: IXpSystem {

    private IEventHub _eventHub;
    
    public XpSystem(IEventHub eventHub) {
        _eventHub = eventHub;

        _eventHub.Subscribe<ResourceHarvestComplete>(OnResourceHarvestComplete);
    }

    private void OnResourceHarvestComplete(ResourceHarvestComplete e) {
        _eventHub.Publish<XpGained>(new XpGained(e.ResourceId, 1));
    }

    public void Dispose() {
        _eventHub.Unsubscribe<ResourceHarvestComplete>(OnResourceHarvestComplete);
    }

}

