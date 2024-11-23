public interface IRandomNumberGenerator {
    public int Next(int maxExclusive);
}

public class RandomNumberGenerator: IRandomNumberGenerator {

    private readonly Random _random;

    public RandomNumberGenerator() {
        _random = new Random();
    }

    public int Next(int maxExclusive) {
        return _random.Next(maxExclusive);
    }

}