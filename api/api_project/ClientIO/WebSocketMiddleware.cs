
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

                    // This sucks but I don't care
                    // These services are only ever used by decoupled events, and never directly injected
                    // So they have to be manually instantiated here
                    var tickSystem = scope.ServiceProvider.GetRequiredService<ITickSystem>();
                    var lootSystem = scope.ServiceProvider.GetRequiredService<ILootSystem>();
                    await using var saveSystem = scope.ServiceProvider.GetRequiredService<ISaveSystem>();
                    var xpSystem = scope.ServiceProvider.GetRequiredService<IXpSystem>();

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

