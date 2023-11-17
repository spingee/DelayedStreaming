// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Shared;
using StackExchange.Redis;

var hostBuilder = Host.CreateApplicationBuilder();
var configuration = hostBuilder.Configuration;

using var host = hostBuilder
                 .UseOrleans((builder) =>
                 {
                     builder.Configure<ClusterOptions>(options =>
                     {
                         options.ClusterId = "test";
                         options.ServiceId = "test";
                     });

                     string redisConnectionString = "redis:6379";
                     var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
                     var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
                     builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);
                     builder.UseRedisClustering(redisConnectionString);

                     builder.AddRedisGrainStorage("PubSubStore", o =>
                     {
                         o.ConfigurationOptions = redisOptions;
                         o.CreateMultiplexer = _ => Task.FromResult<IConnectionMultiplexer>(redisConnectionMultiplexer);
                     });

                     builder.AddMemoryStreams(Constants.SMSProvider)
                            .AddStreaming();
                 })
                 .Build();

host.Start();

var grainFactory = host.Services.GetRequiredService<IGrainFactory>();
await grainFactory.GetGrain<IProducerGrain>(0).Start();
host.WaitForShutdown();