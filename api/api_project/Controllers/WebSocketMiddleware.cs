
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
                    var messageReader = scope.ServiceProvider.GetRequiredService<IClientReader>();
                    var messageWriter = scope.ServiceProvider.GetRequiredService<IClientWriter>();

                    messageWriter.InitSocket(webSocket);
                    await messageReader.ReadMessages(webSocket);
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