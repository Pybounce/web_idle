public record PlayerInventory {
    public string id { get; set; }
    public string UserId { get; set; }
    public Dictionary<int, int> Items { get; set; }
}