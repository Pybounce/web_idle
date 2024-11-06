

public interface IResourceHarvester {
    public bool TryStartResourceHarvest(int resourceId);
    public void StopResourceHarvest(int resourceId);
}

public class ResourceHarvester: IResourceHarvester, IDisposable {
    private int? _resourceId { get; set; }
    private readonly IMessageWriter _messageWriter;
    private readonly IEventHub _eventHub;
    private readonly IScopedTickSystem _scopedTickSystem;
    public ResourceHarvester(
        IScopedTickSystem scopedTickSystem, 
        IMessageWriter messageWriter, 
        IEventHub eventHub) 
        {
        _messageWriter = messageWriter;
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
        Console.WriteLine("harvester ontick");
        if (_resourceId != null) {
            var message = new ItemCollected(_resourceId.Value, 1);
            _messageWriter.AddMessage(message);
        }
    }

    public void Dispose() {
        _scopedTickSystem.OnTick -= OnTick;
    }

}






