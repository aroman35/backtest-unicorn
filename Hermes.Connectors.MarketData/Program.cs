using Flurl.Http;
using Hermes.Connectors.MarketData;
using Hermes.Connectors.MarketData.ExcahngeModels.Binance;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Serilog;
using Websocket.Client;

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
    .UseSerilog((context, builder) => builder.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext())
    .ConfigureServices((context, services) =>
    {
        services
            .AddSingleton(orleansClient)
            .AddOptions()
            .Configure<BinanceConnectionOptions>(context.Configuration.GetSection(nameof(BinanceConnectionOptions)))
            .AddTransient<IWebsocketClient>(provider => new WebsocketClient(new Uri(provider.GetRequiredService<IOptions<BinanceConnectionOptions>>().Value.StreamsUrl)))
            .AddTransient<IFlurlClient>(provider => new FlurlClient(provider.GetRequiredService<IOptions<BinanceConnectionOptions>>().Value.RestUrl))
            .AddSingleton<MarketDataGateway>()
            .AddTransient<OrderBookSnapshotQuery>()
            .AddHostedService<OrderBookService>()
            .AddAutoMapper(typeof(MappingProfile).Assembly);

    })
    .Build();

await host.RunAsync();