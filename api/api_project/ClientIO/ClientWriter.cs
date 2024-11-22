
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public interface IClientWriter {
    public void InitSocket(WebSocket webSocket);
}

public class ClientWriter: IClientWriter, IDisposable {

    private List<byte[]> _messageBuffer;
    private readonly SlimShady _slimShady;
    private WebSocket? _webSocket;
    private readonly IEventHub _eventHub;
    
    public ClientWriter(IEventHub eventHub) {
        _messageBuffer = new List<byte[]>();
        _slimShady = new SlimShady();
        _webSocket = null;
        _eventHub = eventHub;
        _eventHub.Subscribe<Tick>(SendMessages);
        _eventHub.Subscribe<ItemGained>(WriteItemCollectedEvent);
        _eventHub.Subscribe<XpGainedEvent>(WriteXpGainedEvent);
    }

    public void InitSocket(WebSocket webSocket) {
        _webSocket = webSocket;
    }

    private async void SendMessages(Tick tick) {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open) {
            await _slimShady.LockAsync(async () => {
                for (int i = _messageBuffer.Count - 1; i >= 0; i--) {
                    await _webSocket!.SendAsync(new ArraySegment<byte>(_messageBuffer[i]), WebSocketMessageType.Text, true, CancellationToken.None);
                    _messageBuffer.RemoveAt(i);
                }            
            });
        }
        else {
            _webSocket = null;
        }
    }

    public void Dispose() {
        _eventHub.Unsubscribe<ItemGained>(WriteItemCollectedEvent);
        _eventHub.Unsubscribe<XpGainedEvent>(WriteXpGainedEvent);
        _eventHub.Unsubscribe<Tick>(SendMessages);
    }

    private void AddMessage(object data) {
        var jsonString = JsonSerializer.Serialize(data);
        _messageBuffer.Add(Encoding.UTF8.GetBytes(jsonString));
    }

    private void WriteItemCollectedEvent(ItemGained e) {
        AddMessage(new ItemCollected(e.ItemId, e.Amount));
    }
    private void WriteXpGainedEvent(XpGainedEvent e) {
        AddMessage(new XpGained(e.SkillId, e.Amount));
    }

}

