

using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;

public interface IMessageWriter {
    public Task SendMessages(WebSocket webSocket);
}

public class MessageWriter: IMessageWriter {

    private readonly IResourceHarvester _resourceHarvester;
    private const int _writeTickDelay = 1000;

    public MessageWriter(IResourceHarvester resourceHarvester) {
        _resourceHarvester = resourceHarvester;
    }

    public async Task SendMessages(WebSocket webSocket) {
        
        while (webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine("sending!");
            var message = Encoding.UTF8.GetBytes("Message from the server! :D");
            await webSocket.SendAsync(
                new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);

            await Task.Delay(_writeTickDelay);
        }
    }
}