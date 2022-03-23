using AutoMapper;
using Flurl.Http;
using Hermes.Abstractions.GrainInterfaces.MarketData;
using Hermes.Connectors.MarketData.ExcahngeModels.Binance;
using Hermes.MarketData.Abstractions.OrderBook;
using Orleans;
using Utf8Json;
using Websocket.Client;

namespace Hermes.Connectors.MarketData;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly IFlurlClient _flurlClient;
    private readonly HashSet<string> _subscriptions;
    private readonly IWebsocketClient _websocketClient;
    private int _subscriptionId;
    private readonly IMapper _mapper;

    public Worker(ILogger<Worker> logger, IClusterClient clusterClient, IMapper mapper)
    {
        _logger = logger;
        _clusterClient = clusterClient;
        _mapper = mapper;
        _flurlClient = new FlurlClient("https://api.binance.com/api/v3/");
        _subscriptions = new HashSet<string>();
        var url = new Uri($"{connectionOptions.StreamsUrl}");
        _websocketClient = new WebsocketClient(url);
        _websocketClient.ReconnectTimeout = TimeSpan.FromSeconds(10);
        _websocketClient.ReconnectionHappened.Subscribe(info =>
        {
            _websocketClient.Send(GenerateSubscribeMessage(_subscriptions.ToArray()));
            _logger.Warning("Reconnection happened: {Type}", info.Type);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var orderBook = _clusterClient.GetGrain<IOrderBookGrain>("binance/1INCHUSDT");
        var snapshot = await SnapshotQuery();
        
        var grainResponse = await orderBook.TestMethod();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Grain responded: {Response}", grainResponse);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error");
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task<OrderBookSnapshot> SnapshotQuery()
    {
        var response = await _flurlClient
            .Request("depth")
            .SetQueryParam("symbol", "1INCHUSDT")
            .SetQueryParam("limit", 20)
            .AllowAnyHttpStatus()
            .GetAsync();

        if (response.StatusCode != 200)
            throw new ApplicationException("Couldn't receive order-book snapshot");

        var json = await response.GetStringAsync();
        var snapshot = JsonSerializer.Deserialize<OrderBookSnapshotModel>(json);
        return _mapper.Map<OrderBookSnapshot>(snapshot);
    }
}