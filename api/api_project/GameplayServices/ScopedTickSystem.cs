

public interface IScopedTickSystem {
    public void Tick();
    public event Action OnTick;

}

public class ScopedTickSystem: IScopedTickSystem {
    public event Action OnTick;
    public void Tick() {
        Console.WriteLine("Tick");
        OnTick?.Invoke();
    }
}
