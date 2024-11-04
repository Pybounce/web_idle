

using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;

public interface IMessageWriter {
    public void InitSocket(WebSocket webSocket);
    //public bool AddMessage(object data);
}

public class MessageWriter: IMessageWriter {

    //  make a buffer of messages
    private WebSocket _webSocket = null;
    
    public MessageWriter(IScopedTickSystem scopedTickSystem) {
        scopedTickSystem.OnTick += SendMessages;
    }

    public void InitSocket(WebSocket webSocket) {
        _webSocket = webSocket;
    }

    private async void SendMessages() {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open) {
            var message = Encoding.UTF8.GetBytes("Message from the server! :D");
            await _webSocket.SendAsync(
                new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else {
            _webSocket = null;
        }
    }

}

