public interface IRngSystem {
    public int Next(int maxExclusive);
}

public class RngSystem: IRngSystem {

    private readonly Random _random;

    public RngSystem() {
        _random = new Random();
    }

    public int Next(int maxExclusive) {
        return _random.Next(maxExclusive);
    }

}