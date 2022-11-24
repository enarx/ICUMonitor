using ICUMonitor;
using ICUMonitor.Data;

// Create services
var builder = WebApplication.CreateBuilder(args).UseWasiConnectionListener();
builder.Services.AddGrpc();
builder.Services.AddSignalR(); // Simplifies use of WebSockets
builder.Services.AddSingleton<AppData>();

// Construct app instance with required features
var app = builder.Build();
app.UseDefaultFiles().UseBundledStaticFiles();
app.UseRouting();
app.UseGrpcWeb();

// 1. gRPC endpoint called by sensors
app.MapGrpcService<SensorDataReceiverService>().EnableGrpcWeb();

// 2. JSON endpoint called by Svelte UI
app.MapGet("/api/icu", (AppData appData) => appData.ICU);

// 3. Websocket endpoint sends realtime updated to browser
app.MapHub<SensorDataHub>("/sensorDataHub");

app.Run();
