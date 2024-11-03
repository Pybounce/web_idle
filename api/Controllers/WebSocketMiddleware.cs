
public class WebSocketMiddleware {
    
    private readonly RequestDelegate _nextRequestDelegate;
    private readonly IMessageReader _messageReader;
    private readonly IMessageWriter _messageWriter;

    public WebSocketMiddleware(
        RequestDelegate next, 
        IMessageReader messageReader, 
        IMessageWriter messagerWriter
        ) {
        _nextRequestDelegate = next;
        _messageReader = messageReader;
        _messageWriter = messagerWriter;
    }

    public async Task InvokeAsync(HttpContext context) {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var read = _messageReader.ReadMessages(webSocket);
                var write = _messageWriter.SendMessages(webSocket);

                await Task.WhenAll(read, write);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await _nextRequestDelegate(context);
        }
    }
}