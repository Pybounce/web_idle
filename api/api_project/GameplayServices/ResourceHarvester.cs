

public interface IResourceHarvester {
    public bool TryStartResourceHarvest(int resourceId);
    public void StopResourceHarvest(int resourceId);
}

public class ResourceHarvester: IResourceHarvester, IDisposable {
    private int? _resourceId { get; set; }
    private readonly IEventHub _eventHub;

    public ResourceHarvester(IEventHub eventHub) {
        _eventHub = eventHub;
        _eventHub.Subscribe<Tick>(OnTick);
    }

    public bool TryStartResourceHarvest(int resourceId) {
        _resourceId = resourceId;
        return true;
    }

    public void StopResourceHarvest(int resourceId) {
        if (_resourceId == resourceId) {
            _resourceId = null;
        }
    }

    private void OnTick(Tick tick) {
        if (_resourceId != null) {
            _eventHub.Publish(new ResourceHarvestComplete { ResourceId = _resourceId.Value });
        }
    }

    public void Dispose() {
        _eventHub.Unsubscribe<Tick>(OnTick);
    }

}






