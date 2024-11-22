
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