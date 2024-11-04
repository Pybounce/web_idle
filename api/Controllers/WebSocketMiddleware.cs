
public class WebSocketMiddleware {
    
    private readonly RequestDelegate _nextRequestDelegate;
    private readonly IServiceProvider _serviceProvider;

    public WebSocketMiddleware(
        RequestDelegate next, 
        IServiceProvider serviceProvider
        ) {
        _nextRequestDelegate = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context) {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (var scope = _serviceProvider.CreateScope()) {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                var messageReader = scope.ServiceProvider.GetRequiredService<IMessageReader>();
                var messageWriter = scope.ServiceProvider.GetRequiredService<IMessageWriter>();

                var read = messageReader.ReadMessages(webSocket);
                var write = messageWriter.SendMessages(webSocket);

                await Task.WhenAll(read, write);
                }

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