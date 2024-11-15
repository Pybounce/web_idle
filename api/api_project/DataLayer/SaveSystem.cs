


public interface ISaveSystem: IAsyncDisposable, IDisposable {
    // sucks that I am forcing the save system to implement these but I can't be bothered to create a wrapper to optionally do await using var in the web socket middleware
    // so deal with it
}

public class SaveSystem: ISaveSystem  {
    private IScopedTickSystem _tickSystem;
    private IEventHub _eventHub;
    private readonly IDbIO _db;
    
    /// <summary>
    /// Amount of ticks between saves to the db
    /// </summary>
    private int _saveTickDelay = 20;
    private GameState _gameState;
    public SaveSystem(IScopedTickSystem tickSystem, IDbIO db, IEventHub eventHub) {
        _tickSystem = tickSystem;
        _tickSystem.OnTick += OnTick;
        _gameState = new GameState();   //Load state from db here
        _db = db;
        _eventHub = eventHub;

        _eventHub.Subscribe<ItemGained>(OnItemGained);
    }




    public async void OnTick(Tick tick) {
        //save every x ticks using modulo
        if (tick.RawTick % _saveTickDelay == 0) {
            var _ = _db.SavePlayerInventoryAsync(_gameState.PlayerInventory);
        }
    }

    public void Dispose() {
        _tickSystem.OnTick -= OnTick;
    }

    private void OnItemGained(ItemGained e) {
        _gameState.PlayerInventory.AddItem(e.ItemId, e.Amount);
    }

    public async ValueTask DisposeAsync()
    {
        await _db.SavePlayerInventoryAsync(_gameState.PlayerInventory);
    }
}


