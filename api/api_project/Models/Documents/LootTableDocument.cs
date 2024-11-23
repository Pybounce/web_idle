
public record LootTableDocument {
    public required string id { get; set; }
    public required int ResourceId { get; set; }
    public required LootTableItem[] ItemChances { get; set; }
}

public record LootTableItem {
    public required int ItemId { get; set; }
    public required int ChanceDenominator { get; set; }
}