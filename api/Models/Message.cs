
using System.Text.Json.Serialization;

public struct Message {
    [JsonInclude]
    public string MessageType;
    [JsonInclude]
    public object Data;
}