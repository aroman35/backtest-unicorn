using Hermes.Core.Grains.OrderBook;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config
        .AddEnvironmentVariables()
        .AddJsonFile("loggerSettings.json", false))
    .UseSerilog((context, builder) => builder.ReadFrom.Configuration(context.Configuration))
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
            .UsePerfCounterEnvironmentStatistics()
            .AddMemoryGrainStorageAsDefault()
            .AddLogStorageBasedLogConsistencyProvider();
    })
    .Build();

await host.RunAsync();