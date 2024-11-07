

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

        _eventHub.Subscribe<ItemCollectedEvent>(Subbed);
        _eventHub.Subscribe<ItemCollectedEvent>(Subbed2);
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
        //Console.WriteLine("harvester ontick");
        if (_resourceId != null) {
            _eventHub.Publish(new ItemCollectedEvent(_resourceId.Value, 1));

            //var message = new ItemCollected(_resourceId.Value, 1);
            //_messageWriter.AddMessage(message);
        }
    }

    public void Dispose() {
        _scopedTickSystem.OnTick -= OnTick;
        _eventHub.Unsubscribe<ItemCollectedEvent>(Subbed);
        _eventHub.Unsubscribe<ItemCollectedEvent>(Subbed2);
    }

    private void Subbed(ItemCollectedEvent e) {
        Console.WriteLine("item collected event sub");
    }
    private void Subbed2(ItemCollectedEvent e) {
        Console.WriteLine("item collected event sub2");
    }

}






