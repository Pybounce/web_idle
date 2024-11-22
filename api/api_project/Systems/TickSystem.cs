

public interface ITickSystem {

}

public class TickSystem: ITickSystem, IDisposable
{
    private readonly IEventHub _eventHub;
    private Timer? _timer = null;
    private Tick _tick;

    public TickSystem(IEventHub eventHub) {
        _eventHub = eventHub;
        _timer = new Timer(MoveUnits, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        _tick = new Tick();
    }

    private void MoveUnits(object? state) {
        Console.WriteLine("tick " + _tick.RawTick);
        _tick.Next();
        _eventHub.Publish<Tick>(_tick);
    }

    public void Dispose() {
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
    }

}


public struct Tick {
    public int RawTick { get; private set; }

    public Tick(int rawTick) {
        RawTick = rawTick;
    }

    public void Next() {
        this.RawTick += 1;
    }

    public static bool operator >(Tick lhs, Tick rhs) {
        var diff = lhs.RawTick - rhs.RawTick;
        return diff > 0 && diff < int.MaxValue;
    }
    public static bool operator <(Tick lhs, Tick rhs) {
        return !(lhs > rhs);
    }
    public static bool operator ==(Tick lhs, Tick rhs) {
        return lhs.RawTick == rhs.RawTick;
    }
    public static bool operator !=(Tick lhs, Tick rhs) {
        return !(lhs == rhs);
    }

}
