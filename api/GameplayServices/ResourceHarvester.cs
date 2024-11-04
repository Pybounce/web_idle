

public interface IResourceHarvester {
    public bool TryStartResourceHarvest(int resourceId);
    public void StopResourceHarvest(int resourceId);
}

public class ResourceHarvester: IResourceHarvester {
    public int? ResourceId { get; set; }
    private readonly IScopedTickSystem _scopedTickSystem;
    private readonly IMessageWriter _messageWriter;
    public ResourceHarvester(IScopedTickSystem scopedTickSystem, IMessageWriter messageWriter) {
        _scopedTickSystem = scopedTickSystem;
        _messageWriter = messageWriter;
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






