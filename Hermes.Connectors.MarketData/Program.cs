using Hermes.Connectors.MarketData;
using Hermes.Connectors.MarketData.ExcahngeModels.Binance;
using Orleans;
using Orleans.Configuration;

IClusterClient orleansClient = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "hermes";
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await orleansClient.Connect();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(orleansClient);
        services
            .AddHostedService<Worker>()
            .AddAutoMapper(typeof(MappingProfile).Assembly)
            ;

    })
    .Build();

await host.RunAsync();