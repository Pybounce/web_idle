

public interface IScopedTickSystem {
    public void Tick();
    public event Action<Tick> OnTick;

}

public class ScopedTickSystem: IScopedTickSystem {
    public event Action<Tick> OnTick;
    private Tick _tick;

    public ScopedTickSystem() {
        _tick = new Tick();
    }

    public void Tick() {
        _tick.Next();
        Console.WriteLine($"Tick {_tick.RawTick}");
        OnTick?.Invoke(_tick);
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

