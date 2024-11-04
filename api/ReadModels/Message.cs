

using System.Text.Json;

public struct Message {
    public string MessageType { get; set; }
    public JsonElement Data { get; set; }
}