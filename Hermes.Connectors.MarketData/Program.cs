using Hermes.Connectors.MarketData;
using Hermes.Connectors.MarketData.ExcahngeModels.Binance;
using Orleans;
using Orleans.Configuration;
using Serilog;

IClusterClient orleansClient = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "hermes";
    })
    .Build();

await orleansClient.Connect();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config
        .AddEnvironmentVariables()
        .AddJsonFile("loggerSettings.json", false))
    .UseSerilog((context, builder) => builder.ReadFrom.Configuration(context.Configuration))
    .ConfigureServices(services =>
    {
        services.AddSingleton(orleansClient);
        services
            .AddHostedService<Worker>()
            .AddAutoMapper(typeof(MappingProfile).Assembly);

    })
    .Build();

await host.RunAsync();