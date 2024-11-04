

using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

public interface IMessageWriter {
    public void InitSocket(WebSocket webSocket);
    public void AddMessage(object data);
}

public class MessageWriter: IMessageWriter {

    private List<byte[]> _messageBuffer;
    private readonly SemaphoreSlim _sendSemaphore;
    private WebSocket _webSocket;
    
    public MessageWriter(IScopedTickSystem scopedTickSystem) {
        scopedTickSystem.OnTick += SendMessages;
        _messageBuffer = new List<byte[]>();
        _sendSemaphore = new SemaphoreSlim(1, 1);
        _webSocket = null;
    }

    public void InitSocket(WebSocket webSocket) {
        _webSocket = webSocket;
    }

    public void AddMessage(object data) {
        var jsonString = JsonSerializer.Serialize(data);
        _messageBuffer.Add(Encoding.UTF8.GetBytes(jsonString));
    }

    private async void SendMessages() {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open) {
            await _sendSemaphore.WaitAsync();
            try {
                for (int i = _messageBuffer.Count - 1; i >= 0; i--) {
                    await SendMessageAtIndex(i);
                    _messageBuffer.RemoveAt(i);
                }
            }
            finally {
                _sendSemaphore.Release();
            }
        }
        else {
            _webSocket = null;
        }
    }

    private async Task SendMessageAtIndex(int index) {
        await _webSocket.SendAsync(new ArraySegment<byte>(_messageBuffer[index]), WebSocketMessageType.Text, true, CancellationToken.None);
    }

}

