

public interface IEventHub {
    public void Add<T>(T e) where T : IEvent;
}

public class EventHub: IEventHub, IDisposable {

    private List<IEvent> _events;
    private readonly IMessageWriter _messageWriter;
    private readonly IScopedTickSystem _tickSystem;
    public EventHub(IScopedTickSystem tickSystem, IMessageWriter messageWriter) {
        _events = new List<IEvent>();
        _messageWriter = messageWriter;
        _tickSystem = tickSystem;
        _tickSystem.OnTick += OnTick;
    }


    public void Add<T>(T e) where T : IEvent {
        _events.Add(e);
    }

    private void OnTick() {
        Console.WriteLine("event hub tick");
    }

    public void Dispose() {
        _tickSystem.OnTick -= OnTick;
    }

}