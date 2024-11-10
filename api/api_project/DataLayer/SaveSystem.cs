

public interface ISaveSystem {

}

public class SaveSystem: ISaveSystem {
    private IScopedTickSystem _tickSystem;
    /// <summary>
    /// Amount of ticks between saves to the db
    /// </summary>
    private int _saveTickDelay = 100;
    private GameState _gameState;

    public SaveSystem(IScopedTickSystem tickSystem) {
        _tickSystem = tickSystem;
        _gameState = new GameState();   //Load state from db here
    }
}


