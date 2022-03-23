using Hermes.Core.Grains.OrderBook;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, builder) =>
    {
        builder
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "hermes";
            })
            .ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(OrderBookGrain).Assembly)
                .WithReferences())
            .ConfigureLogging(logging => logging.AddConsole())
            .UsePerfCounterEnvironmentStatistics()
            .AddMemoryGrainStorageAsDefault()
            .AddLogStorageBasedLogConsistencyProvider();
    })
    .Build();

await host.RunAsync();