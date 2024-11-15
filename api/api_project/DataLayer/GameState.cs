public struct GameState {
    public Inventory PlayerInventory { get; set; }

    public GameState() {
        PlayerInventory = new Inventory();
    }
}