

public interface IResourceHarvester {
    public bool TryStartResourceHarvest(int resourceId);
    public void StopResourceHarvest(int resourceId);
}

public class ResourceHarvester: IResourceHarvester {
    private int? _resourceId { get; set; }
    private readonly IScopedTickSystem _scopedTickSystem;
    private readonly IMessageWriter _messageWriter;
    public ResourceHarvester(IScopedTickSystem scopedTickSystem, IMessageWriter messageWriter) {
        _scopedTickSystem = scopedTickSystem;
        _messageWriter = messageWriter;
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
            var message = new ItemCollected {
                ItemId = 1,
                Amount = 1
            };
            _messageWriter.AddMessage(message);
        }
        Console.WriteLine("resource harvester on tick");
    }
}






