

public interface ISaveSystem {

}

public class SaveSystem: ISaveSystem, IDisposable {
    private IScopedTickSystem _tickSystem;
    /// <summary>
    /// Amount of ticks between saves to the db
    /// </summary>
    private GameState _gameState;
    public SaveSystem(IScopedTickSystem tickSystem) {
        _tickSystem = tickSystem;
        _tickSystem.OnTick += OnTick;
        _gameState = new GameState();   //Load state from db here
    }




    public async void OnTick(Tick tick) {
        //save every x ticks using modulo

    }

    public void Dispose() {
        _tickSystem.OnTick -= OnTick;
    }

}


