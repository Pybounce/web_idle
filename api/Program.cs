using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMessageReader, MessageReader>();
builder.Services.AddScoped<IMessageWriter, MessageWriter>();

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

app.UseMiddleware<WebSocketMiddleware>();

//do app.Use((context, next) => {}) to handle auth
// or use app.UseMiddlewear<MyAuthMiddlewear>(); ??
// I can then pass the context to the next middlewear with await next(context);

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
