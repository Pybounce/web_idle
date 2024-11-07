
public struct ItemCollectedEvent: IEvent
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public ItemCollectedEvent(int itemId, int amount) {
        ItemId = itemId;
        Amount = amount;
    }
}