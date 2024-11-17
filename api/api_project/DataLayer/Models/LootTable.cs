
public record LootTable {
    public required string id { get; set; }
    public required int ResourceId { get; set; }
    public required LootTableItem[] ItemChances { get; set; }
}

public record LootTableItem {
    public int ItemId { get; set; }
    public int ChanceDenominator { get; set; }
}