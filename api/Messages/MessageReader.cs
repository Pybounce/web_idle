using System.Net.WebSockets;
using System.Text;
using System.Text.Json;


public interface IMessageReader {
    public Task ReadMessages(WebSocket webSocket);
}

public class MessageReader: IMessageReader {

    private readonly IResourceHarvester _playerActionManager;

    public MessageReader(IResourceHarvester playerActionManager) {
        _playerActionManager = playerActionManager;
    }

    public async Task ReadMessages(WebSocket webSocket) {
        try {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
                string jsonStr = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                Console.WriteLine("________________________");
                Console.WriteLine("");
                Console.WriteLine("Message recieved");
                HandleMessage(jsonStr);
            }

            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);

        }
        catch(Exception e)
        {
            Console.WriteLine("error: " + e.Message);
        }

    }

    private void HandleMessage(string messageJson) {
        var message = MessageFromJson(messageJson);
        switch(message.MessageType) {
            case MessageTypes.StartResourceHarvest:
                //var startResourceHarvestMessage = (StartResourceHarvest)message.Data;
                //_playerActionManager.TryStartResourceHarvest(startResourceHarvestMessage.ResourceId);
                Console.WriteLine("Start resource harvest");
                break;
            case MessageTypes.StopResourceHarvest:
                //var stopResourceHarvestMessage = (StopResourceHarvest)message.Data;
                //_playerActionManager.StopResourceHarvest(stopResourceHarvestMessage.ResourceId);
                Console.WriteLine("End resource harvest");
                break;
            default:
                Console.WriteLine("you fucked up.");
                break;
        }
    }

    private Message MessageFromJson(string messageJson) {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize<Message>(messageJson, options);
    }

}

