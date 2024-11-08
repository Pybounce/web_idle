
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public interface IMessageWriter {
    public void InitSocket(WebSocket webSocket);
    public void AddMessage(object data);
}

public class MessageWriter: IMessageWriter, IDisposable {

    private List<byte[]> _messageBuffer;
    private readonly SemaphoreSlim _sendSemaphore;
    private WebSocket? _webSocket;
    private readonly IEventHub _eventHub;
    
    public MessageWriter(IScopedTickSystem scopedTickSystem, IEventHub eventHub) {
        scopedTickSystem.OnTick += SendMessages;
        _messageBuffer = new List<byte[]>();
        _sendSemaphore = new SemaphoreSlim(1, 1);
        _webSocket = null;
        _eventHub = eventHub;
        _eventHub.Subscribe<ItemCollectedEvent>(DoSomething);
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
                    await _webSocket!.SendAsync(new ArraySegment<byte>(_messageBuffer[i]), WebSocketMessageType.Text, true, CancellationToken.None);
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

    private void DoSomething(ItemCollectedEvent itemCollectedEvent) {
        Console.WriteLine("wapow reading event");
        _eventHub.Unsubscribe<ItemCollectedEvent>(DoSomething);
    }

    public void Dispose() {
        _eventHub.Unsubscribe<ItemCollectedEvent>(DoSomething);
    }

}

