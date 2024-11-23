
public struct ItemGainedEvent: IEvent
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public ItemGainedEvent(int itemId, int amount) {
        ItemId = itemId;
        Amount = amount;
    }
}