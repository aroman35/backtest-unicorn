using System.Collections.Immutable;
using Hermes.Abstractions.GrainInterfaces.MarketData;
using Hermes.MarketData.Abstractions.OrderBook;
using Orleans.EventSourcing;
using Orleans.Providers;
using Serilog;

namespace Hermes.Core.Grains.OrderBook;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class OrderBookGrain : JournaledGrain<OrderBookState, OrderBookEventBase>, IOrderBookGrain
{
    private readonly ILogger _logger;

    public OrderBookGrain(ILogger logger)
    {
        _logger = logger.ForContext<OrderBookGrain>();
    }

    public Task AddSnapshot(OrderBookSnapshot orderBookSnapshot)
    {
        _logger.Information("Order-book snapshot added");
        RaiseEvent(new AddOrderBookSnapshotEvent(orderBookSnapshot));
        return Task.CompletedTask;
    }

    public Task ApplyDifference(OrderBookDiffModel orderBookDiffModel)
    {
        _logger.Information("Order-book update received");
        RaiseEvent(new AddOrderBookDifference(orderBookDiffModel));
        return Task.CompletedTask;
    }

    public Task<SortedDictionary<decimal, decimal>> Asks()
    {
        return Task.FromResult(State.Asks);
    }

    public Task<SortedDictionary<decimal, decimal>> Bids()
    {
        return Task.FromResult(State.Bids);
    }

    public Task<OrderBookEntry> BestBid()
    {
        return Task.FromResult(State.BestBid);
    }

    public Task<OrderBookEntry> BestAsk()
    {
        return Task.FromResult(State.BestAsk);
    }

    public Task<string> TestMethod()
    {
        return Task.FromResult("OK");
    }
}

public class OrderBookState
{
    public SortedDictionary<decimal, decimal> Asks { get; }
    public SortedDictionary<decimal, decimal> Bids { get; }

    public OrderBookEntry BestBid => Bids.Any() ? new OrderBookEntry(Bids.LastOrDefault()) : OrderBookEntry.Empty;
    public OrderBookEntry BestAsk => Asks.Any() ? new OrderBookEntry(Asks.FirstOrDefault()) : OrderBookEntry.Empty;
    
    public bool IsBuilt { get; private set; }

    public OrderBookState()
    {
        Asks = new SortedDictionary<decimal, decimal>();
        Bids = new SortedDictionary<decimal, decimal>();
        _cachedSnapshots = new SortedSet<OrderBookCache>();
    }

    private readonly SortedSet<OrderBookCache> _cachedSnapshots;

    public void Apply(AddOrderBookDifference differenceEvent)
    {
        var cache = new OrderBookCache(differenceEvent.Difference.FirstUpdateId, differenceEvent.Difference.FinalUpdateId, differenceEvent.Difference.Bids, differenceEvent.Difference.Asks);
        if (!IsBuilt)
        {
            _cachedSnapshots.Add(cache);
        }
        else
        {
            Merge(cache);
        }
    }

    public void Apply(AddOrderBookSnapshotEvent snapshotEvent)
    {
        _cachedSnapshots.RemoveWhere(x => x.FinalUpdateId <= snapshotEvent.Snapshot.LastUpdateId);
        
        foreach (var snapshotAsk in snapshotEvent.Snapshot.Asks)
            Asks[snapshotAsk[0]] = snapshotAsk[1];

        foreach (var snapshotBid in snapshotEvent.Snapshot.Bids)
            Bids[snapshotBid[0]] = snapshotBid[1];
        
        foreach (var orderBookCache in _cachedSnapshots)
        {
            Merge(orderBookCache);
        }
        IsBuilt = true;
    }

    private void Merge(OrderBookCache cache)
    {
        foreach (var (price, quantity) in cache.Bids)
        {
            if (quantity == 0)
            {
                Bids.Remove(price);
                continue;
            }
            Bids[price] = quantity;
        }

        foreach (var (price, quantity) in cache.Asks)
        {
            if (quantity == 0)
            {
                Asks.Remove(price);
                continue;
            }
            Asks[price] = quantity;
        }
    }
}

public class OrderBookCache : IComparable<OrderBookCache>
{
    public OrderBookCache(long firstUpdateId, long finalUpdateId, decimal[][] bids, decimal[][] asks)
    {
        FirstUpdateId = firstUpdateId;
        FinalUpdateId = finalUpdateId;
        Bids = bids.ToImmutableSortedDictionary(x => x[0], x => x[1]);
        Asks = asks.ToImmutableSortedDictionary(x => x[0], x => x[1]);
    }
    public long FirstUpdateId { get; }
    public long FinalUpdateId { get; }
    public ImmutableSortedDictionary<decimal, decimal> Bids { get; }
    public ImmutableSortedDictionary<decimal, decimal> Asks { get; }

    public int CompareTo(OrderBookCache other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return FinalUpdateId.CompareTo(other.FinalUpdateId);
    }
}

public abstract record OrderBookEventBase;

public record AddOrderBookSnapshotEvent(OrderBookSnapshot Snapshot) : OrderBookEventBase;

public record AddOrderBookDifference(OrderBookDiffModel Difference) : OrderBookEventBase;