using System.Runtime.Serialization;

namespace Hermes.Connectors.MarketData.ExcahngeModels.Binance;

public class TradesSubscriptionModel
{
    [DataMember(Name = "method")]
    public WsMethod Method { get; init; }
    [DataMember(Name = "params")]
    public string[] Params { get; init; }
    [DataMember(Name = "id")]
    public int Id { get; init; }
}

public enum WsMethod
{
    SUBSCRIBE,
    UNSUBSCRIBE
}