using System.Reactive.Linq;
using System.Runtime.Serialization;
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
    private int _subscriptionId = 0;
    private IDisposable _subscription;
    private readonly IMapper _mapper;

    public Worker(ILogger<Worker> logger, IClusterClient clusterClient, IMapper mapper)
    {
        _logger = logger;
        _clusterClient = clusterClient;
        _mapper = mapper;
        _flurlClient = new FlurlClient("https://api.binance.com/api/v3/");
        _subscriptions = new HashSet<string>();
        var url = new Uri("wss://stream.binance.com:9443/stream");
        _websocketClient = new WebsocketClient(url);
        _websocketClient.ReconnectTimeout = TimeSpan.FromSeconds(10);
        _websocketClient.ReconnectionHappened.Subscribe(info =>
        {
            _logger.LogWarning("Reconnection happened: {Type}", info.Type);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var orderBook = _clusterClient.GetGrain<IOrderBookGrain>("binance/BTCUSDT");
            _subscription = SubscribeOnOrderBookStream("BTCUSDT", diff => orderBook.ApplyDifference(diff));
            //_subscription = SubscribeOnOrderBookStream("1INCHUSDT", diff => Task.Run(() => _logger.LogInformation("Order book update received")));
            var snapshot = await SnapshotQuery();
            await orderBook.AddSnapshot(snapshot);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error");
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task<OrderBookSnapshot> SnapshotQuery()
    {
        var response = await _flurlClient
            .Request("depth")
            .SetQueryParam("symbol", "BTCUSDT")
            .SetQueryParam("limit", 20)
            .AllowAnyHttpStatus()
            .GetAsync();

        if (response.StatusCode != 200)
            throw new ApplicationException("Couldn't receive order-book snapshot");

        var json = await response.GetStringAsync();
        var snapshot = JsonSerializer.Deserialize<OrderBookSnapshotModel>(json);
        return _mapper.Map<OrderBookSnapshot>(snapshot);
    }

    private IDisposable SubscribeOnOrderBookStream(string symbol, Func<OrderBookDiffModel, Task> onMessage)
    {
        var streamName = $"{symbol.ToLowerInvariant()}@depth@100ms";
        var subscription = _websocketClient
            .MessageReceived
            .Where(message => !string.IsNullOrEmpty(message.Text))
            .Where(message => message.Text.Contains($"\"stream\":\"{streamName}\""))
            .Select(message => JsonSerializer.Deserialize<StreamMessage<OrderBookDiffExchangeModel>>(message.Text).Data)
            .Select(trade => _mapper.Map<OrderBookDiffModel>(trade))
            .Select(diff => Observable.FromAsync(async () => await onMessage(diff)))
            .Concat()
            .Subscribe(msg => _logger.LogInformation("Order book update received"));

        var subscribeMessage = GenerateSubscribeMessage(new []{streamName});
        _websocketClient.Send(subscribeMessage);
        _subscriptions.Add(streamName);
        _logger.LogInformation("Connected {Symbol} on order-book updates stream", symbol);
        
        return subscription;
    }
    
    private string GenerateSubscribeMessage(string[] streamNames)
    {
        var model = new TradesSubscriptionModel()
        {
            Method = WsMethod.SUBSCRIBE,
            Params = streamNames,
            Id = ++_subscriptionId
        };
        var json = JsonSerializer.ToJsonString(model);
        return json;
    }

    public override void Dispose()
    {
        _subscription.Dispose();
        base.Dispose();
    }
}

public class StreamMessage<TMessage>
{
    [DataMember(Name = "stream")]
    public string Stream { get; set; }
    [DataMember(Name = "data")]
    public TMessage Data { get; set; }
}