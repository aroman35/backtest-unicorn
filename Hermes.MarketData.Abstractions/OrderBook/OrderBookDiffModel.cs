namespace Hermes.MarketData.Abstractions.OrderBook;

public class OrderBookDiffModel
{
    public DateTime Timestamp { get; set; }
    public long FirstUpdateId { get; set; }
    public long FinalUpdateId { get; set; }
    public decimal[][] Bids { get; set; }
    public decimal[][] Asks { get; set; }
}

public class OrderBookSnapshot
{
    public long LastUpdateId { get; set; }
    public decimal[][] Bids { get; set; }
    public decimal[][] Asks { get; set; }
}

public class OrderBookEntry
{
    private OrderBookEntry()
    {
    }

    public static OrderBookEntry Empty => new OrderBookEntry()
    {
        Price = 0,
        Quantity = 0
    };
    
    public OrderBookEntry(KeyValuePair<decimal, decimal> entry)
    {
        var (price, quantity) = entry;
        Price = price;
        Quantity = quantity;
    }
    public decimal Price { get; private init; }
    public decimal Quantity { get; private init;}
}