// See https://aka.ms/new-console-template for more information

using Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Streams;
using Shared;

var hostBuilder = Host.CreateDefaultBuilder();
hostBuilder.ConfigureServices(collection =>
{
    collection.AddOrleansClient(clientBuilder =>
    {
        clientBuilder.AddMemoryStreams(Constants.SMSProvider);
        clientBuilder.AddStreaming();
        clientBuilder.UseConnectionRetryFilter<ClientConnectionRetryFilter>();

        clientBuilder.Configure<ClusterOptions>(opt =>
        {
            opt.ClusterId = "test";
            opt.ServiceId = "test";
        });

        clientBuilder.UseRedisClustering("redis:6379");
    });
});


IHost host = hostBuilder.Build();
host.Start();

var clusterClient = host.Services.GetRequiredService<IClusterClient>();
var streamProvider = clusterClient.GetStreamProvider(Constants.SMSProvider);
var stream = streamProvider.GetStream<Message>(Constants.StreamName,0);
await stream.SubscribeAsync((message, token) =>
{
    Console.WriteLine($"{DateTime.Now} - Received message: {message}");
    return Task.CompletedTask;
});
Console.WriteLine($"Streaming started {DateTime.Now}");

host.WaitForShutdown();