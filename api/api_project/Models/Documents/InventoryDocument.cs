public record InventoryDocument {
    public required string id { get; set; }
    public required string UserId { get; set; }
    public required Dictionary<int, int> Items { get; set; }
}