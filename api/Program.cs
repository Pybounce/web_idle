using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(b => b
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
}

app.UseWebSockets(new WebSocketOptions {
    KeepAliveInterval = TimeSpan.FromMinutes(2)
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var read = ReadMessages(webSocket);
            var send = SendMessages(webSocket);

            await Task.WhenAll(read, send);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }

});


static async Task ReadMessages(WebSocket webSocket)
{
    Console.WriteLine("start reading!");
    var buffer = new byte[1024 * 4];
    var receiveResult = await webSocket.ReceiveAsync(
        new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!receiveResult.CloseStatus.HasValue)
    {
        receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);

}

static async Task SendMessages(WebSocket webSocket)
{
    Console.WriteLine("start sending!");
    var buffer = new byte[1024 * 4];

    while (webSocket.State == WebSocketState.Open)
    {
        Console.WriteLine("sending!");
        var message = Encoding.UTF8.GetBytes("Message from the server! :D");
        await webSocket.SendAsync(
            new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);

        await Task.Delay(5000);
    }


}

//do app.Use((context, next) => {}) to handle auth
// or use app.UseMiddlewear<MyAuthMiddlewear>(); ??
// I can then pass the context to the next middlewear with await next(context);

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
