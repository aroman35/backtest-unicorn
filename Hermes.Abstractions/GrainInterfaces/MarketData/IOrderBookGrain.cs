using Hermes.MarketData.Abstractions.OrderBook;
using Orleans;

namespace Hermes.Abstractions.GrainInterfaces.MarketData;

public interface IOrderBookGrain : IGrainWithStringKey
{
    Task AddSnapshot(OrderBookSnapshot orderBookSnapshot);
    Task<SortedDictionary<decimal, decimal>> Asks();
    Task<SortedDictionary<decimal, decimal>> Bids();
    Task<OrderBookEntry> BestBid();
    Task<OrderBookEntry> BestAsk();
    Task<string> TestMethod();
}