
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<TickSystem>();
builder.Services.AddHostedService<HostedDataService>();

builder.Services.AddSingleton<IScopedTickSystem, ScopedTickSystem>();
builder.Services.AddScoped<IEventHub, EventHub>();
builder.Services.AddScoped<ILootSystem, LootSystem>();
builder.Services.AddScoped<IResourceHarvester, ResourceHarvester>();
builder.Services.AddScoped<IClientReader, ClientReader>();
builder.Services.AddScoped<IClientWriter, ClientWriter>();
builder.Services.AddScoped<ISaveSystem, SaveSystem>();
builder.Services.AddScoped<IXpSystem, XpSystem>();
builder.Services.AddScoped<IGameDb, GameDb>();

builder.Services.AddSingleton<ILootDataService, LootDataService>();

builder.Services.AddSingleton<ICosmosClientFactory>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("CosmosDb");
    if (connectionString == null) {
        throw new NullReferenceException("cosmos connection string");
    }
    return new CosmosClientFactory(connectionString);
});

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
