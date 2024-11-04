

public struct ItemCollected {
    public ItemCollected(int itemId, int amount) {
        MessageType = WriteMessageTypes.ItemCollected;
        ItemId = itemId;
        Amount = amount;
    }
    public string MessageType { get; set; }
    public int ItemId { get; set; }
    public int Amount { get; set; }
}

