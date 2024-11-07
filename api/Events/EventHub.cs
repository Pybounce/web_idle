


public interface IEventHub {
    public void Publish<T>(T e);
    public void Subscribe<T>(Action<T> handler);
    public void Unsubscribe<T>(Action<T> handler);
}

public class EventHub: IEventHub, IDisposable {

    private readonly Dictionary<Type, Delegate?> _subscribers;
    
    public EventHub() {
        _subscribers = new Dictionary<Type, Delegate?>();
    }

    public void Publish<T>(T e) {
        if (_subscribers.TryGetValue(typeof(T), out Delegate? d)) {
            if (d == null) { return; }
            var action = d as Action<T>;
            action?.Invoke(e);
        }
    }

    public void Subscribe<T>(Action<T> handler) {
        Delegate newDelegate = (T e) => {};
        if (!_subscribers.ContainsKey(typeof(T))) {
            _subscribers.Add(typeof(T), newDelegate);
        }
        if (_subscribers.TryGetValue(typeof(T), out Delegate? d)) {
            if (d == null) {
                d = newDelegate;
            }
            _subscribers[typeof(T)] = Delegate.Combine(d, handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler) {
        if (_subscribers.TryGetValue(typeof(T), out Delegate? d)) {
            if (d == null) { return; }
            _subscribers[typeof(T)] = Delegate.Remove(d, handler);
        }
    }

    public void Dispose() {
        _subscribers.Clear();
    }

}