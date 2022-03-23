using System.Runtime.Serialization;

namespace Hermes.Connectors.MarketData.ExcahngeModels.Binance;

/// <summary>
/// <see href="https://binance-docs.github.io/apidocs/spot/en/#order-book"/>
/// </summary>
public class OrderBookSnapshotModel
{
    [DataMember(Name = "lastUpdateId")]
    public long LastUpdateId { get; set; }
    [DataMember(Name = "bids")]
    public decimal[][] Bids { get; set; }
    [DataMember(Name = "asks")]
    public decimal[][] Asks { get; set; }
}