
public interface IXpSystem {

}

public class XpSystem: IXpSystem {

    private IEventHub _eventHub;
    
    public XpSystem(IEventHub eventHub) {
        _eventHub = eventHub;

        _eventHub.Subscribe<ResourceHarvestCompleteEvent>(OnResourceHarvestComplete);
    }

    private void OnResourceHarvestComplete(ResourceHarvestCompleteEvent e) {
        _eventHub.Publish<XpGainedEvent>(new XpGainedEvent(e.ResourceId, 1));
    }

    public void Dispose() {
        _eventHub.Unsubscribe<ResourceHarvestCompleteEvent>(OnResourceHarvestComplete);
    }

}

