

public interface IResourceHarvester {
    public bool TryStartResourceHarvest(int resourceId);
    public void StopResourceHarvest(int resourceId);
}

public class ResourceHarvester: IResourceHarvester {
    public int? ResourceId { get; set; }
    private readonly IScopedTickSystem _scopedTickSystem;
    public ResourceHarvester(IScopedTickSystem scopedTickSystem) {
        Console.WriteLine("register");
        _scopedTickSystem = scopedTickSystem;
        _scopedTickSystem.OnTick += OnTick;
    }

    public bool TryStartResourceHarvest(int resourceId) {

        this.ResourceId = resourceId;
        return true;
    }
    public void StopResourceHarvest(int resourceId) {

        if (ResourceId == resourceId) {
            ResourceId = null;
        }
    }

    private void OnTick() {
        Console.WriteLine("resource harvester on tick");
    }
}


