using System.Runtime.Serialization;

namespace Hermes.Connectors.MarketData.ExcahngeModels.Binance;

/// <summary>
/// <see href="https://binance-docs.github.io/apidocs/spot/en/#diff-depth-stream"/>
/// </summary>
public class OrderBookDiffExchangeModel
{
    [DataMember(Name = "e")]
    public string EventType { get; set; }
    [DataMember(Name = "E")]
    public long EventTime { get; set; }
    [DataMember(Name = "s")]
    public string Symbol { get; set; }
    [DataMember(Name = "U")]
    public long FirstUpdateId { get; set; }
    [DataMember(Name = "u")]
    public long FinalUpdateId { get; set; }
    [DataMember(Name = "b")]
    public decimal[][] Bids { get; set; }
    [DataMember(Name = "a")]
    public decimal[][] Asks { get; set; }
}