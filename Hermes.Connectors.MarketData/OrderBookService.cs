using Hermes.Abstractions.GrainInterfaces.MarketData;
using Hermes.MarketData.Abstractions.OrderBook;
using Microsoft.Extensions.Options;
using Orleans;
using ILogger = Serilog.ILogger;

namespace Hermes.Connectors.MarketData;

public class OrderBookService : BackgroundService
{
    private readonly MarketDataGateway _marketDataGateway;
    private readonly OrderBookSnapshotQuery _orderBookSnapshotQuery;
    private readonly IClusterClient _clusterClient;
    private readonly string _symbol;
    private readonly ILogger _logger;

    private IDisposable _subscription;
    private Task _socketGw;

    public OrderBookService(
        MarketDataGateway marketDataGateway,
        OrderBookSnapshotQuery orderBookSnapshotQuery,
        IClusterClient clusterClient,
        ILogger logger,
        IOptions<BinanceConnectionOptions> options)
    {
        _marketDataGateway = marketDataGateway;
        _orderBookSnapshotQuery = orderBookSnapshotQuery;
        _clusterClient = clusterClient;
        _symbol = options.Value.MdSymbol;
        _logger = logger
            .ForContext<OrderBookService>()
            .ForContext("Symbol", _symbol);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _socketGw = _marketDataGateway.Start();
        return base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_socketGw is not null) await _socketGw;
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var orderBook = _clusterClient.GetGrain<IOrderBookGrain>(_symbol);
        _subscription = _marketDataGateway.SubscribeOnOrderBookUpdates(_symbol, message => orderBook.ApplyDifference(message));
        _logger.Information("Started distribution");
        var snapshot = await _orderBookSnapshotQuery.Handle(_symbol);
        _logger.Information("Received snapshot");
        await orderBook.AddSnapshot(snapshot);
    }
    
    public override void Dispose()
    {
        _subscription?.Dispose();
        base.Dispose();
    }
}