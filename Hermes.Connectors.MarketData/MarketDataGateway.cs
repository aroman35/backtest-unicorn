using System.Reactive.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Hermes.Connectors.MarketData.ExcahngeModels.Binance;
using Hermes.MarketData.Abstractions.OrderBook;
using Utf8Json;
using Websocket.Client;
using ILogger = Serilog.ILogger;

namespace Hermes.Connectors.MarketData;

public class MarketDataGateway
{
    private readonly HashSet<string> _subscriptions;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IWebsocketClient _websocketClient;
    private int _subscriptionId;

    public MarketDataGateway(
        IWebsocketClient websocketClient,
        IMapper mapper,
        ILogger logger)
    {
        _mapper = mapper;
        _logger = logger.ForContext<MarketDataGateway>();
        _subscriptions = new HashSet<string>();
        _websocketClient = websocketClient;
        _websocketClient.ReconnectTimeout = TimeSpan.FromSeconds(10);
        _websocketClient.ReconnectionHappened.Subscribe(info =>
        {
            _websocketClient.Send(GenerateSubscribeMessage(_subscriptions.ToArray()));
            _logger.Warning("Reconnection happened: {Type}", info.Type);
        });
    }

    public async Task Start()
    {
        if (!_websocketClient.IsStarted)
            await _websocketClient.Start();
        _logger.Information("Websocket connected to {Url}", _websocketClient.Url);
    }
    
    public IDisposable SubscribeOnOrderBookUpdates(string symbol, Func<OrderBookDiffModel, Task> onMessage)
    {
        var streamName = $"{symbol.ToLowerInvariant()}@depth@100ms";
        var subscription = _websocketClient
            .MessageReceived
            .Where(message => !string.IsNullOrEmpty(message.Text))
            .Where(message => message.Text.Contains($"\"stream\":\"{streamName}\""))
            .Select(message => JsonSerializer.Deserialize<StreamMessage<OrderBookDiffExchangeModel>>(message.Text).Data)
            .Select(trade => _mapper.Map<OrderBookDiffModel>(trade))
            .Select(diff => Observable.FromAsync(async () => await onMessage(diff)))
            .Merge()
            .Subscribe();
        
        var subscribeMessage = GenerateSubscribeMessage(new []{streamName});
        _websocketClient.Send(subscribeMessage);
        _subscriptions.Add(streamName);
        _logger.Information("Connected {Symbol} on order-book updates stream", symbol);
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
}

public class StreamMessage<TMessage>
{
    [DataMember(Name = "stream")]
    public string Stream { get; set; }
    [DataMember(Name = "data")]
    public TMessage Data { get; set; }
}