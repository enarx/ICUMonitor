using System.Net.Http.Json;
using ICUMonitor.Data;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using SensorSimulator;

var BackendHost = args.Length >= 2 ? args[1] : "http://localhost:5000";

// First get the existing data
using var httpClient = new HttpClient();
var icu = await httpClient.GetFromJsonAsync<Building>($"{BackendHost}/api/icu");

// Prepare a gRPC client
using var httpClientHandler = new HttpClientHandler();
using var channel = GrpcChannel.ForAddress(BackendHost, new GrpcChannelOptions { HttpHandler = new GrpcWebHandler(httpClientHandler) { HttpVersion = new Version(1, 1) } });
var client = new SensorDataReceiver.SensorDataReceiverClient(channel);

// Keep sending simulated data until ctrl+c
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => { e.Cancel = true; cts.Cancel(); };
while (!cts.IsCancellationRequested)
{
    var dataBatch = DataGenerator.NextDataBatch(icu!);
    var result = await client.StoreDataBatchAsync(dataBatch);
    Console.WriteLine($"Sent batch. Result OK: { result.Ok }");
    await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(1), cts.Token));
}
