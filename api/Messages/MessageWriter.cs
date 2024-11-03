

using System.Net.WebSockets;
using System.Text;

public interface IMessageWriter {
    public Task SendMessages(WebSocket webSocket);
}

public class MessageWriter: IMessageWriter {
    public async Task SendMessages(WebSocket webSocket) {
        
        while (webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine("sending!");
            var message = Encoding.UTF8.GetBytes("Message from the server! :D");
            await webSocket.SendAsync(
                new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);

            await Task.Delay(5000);
        }
    }
}