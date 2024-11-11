

public interface IResourceHarvester {
    public bool TryStartResourceHarvest(int resourceId);
    public void StopResourceHarvest(int resourceId);
}

public class ResourceHarvester: IResourceHarvester, IDisposable {
    private int? _resourceId { get; set; }
    private readonly IEventHub _eventHub;
    private readonly IScopedTickSystem _scopedTickSystem;

    public ResourceHarvester(
        IScopedTickSystem scopedTickSystem, 
        IEventHub eventHub) 
        {
        _eventHub = eventHub;
        _scopedTickSystem = scopedTickSystem;
        _scopedTickSystem.OnTick += OnTick;
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

    private void OnTick() {
        if (_resourceId != null) {
            _eventHub.Publish(new ResourceHarvestComplete { ResourceId = _resourceId.Value });
        }
    }

    public void Dispose() {
        _scopedTickSystem.OnTick -= OnTick;
    }

}






