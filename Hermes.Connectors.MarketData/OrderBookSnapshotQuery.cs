using AutoMapper;
using Flurl.Http;
using Hermes.Connectors.MarketData.ExcahngeModels.Binance;
using Hermes.MarketData.Abstractions.OrderBook;
using Utf8Json;

namespace Hermes.Connectors.MarketData;

public class OrderBookSnapshotQuery
{
    private readonly IMapper _mapper;
    private readonly IFlurlClient _flurlClient;

    public OrderBookSnapshotQuery(IMapper mapper, IFlurlClient flurlClient)
    {
        _mapper = mapper;
        _flurlClient = flurlClient;
    }

    public async Task<OrderBookSnapshot> Handle(string symbol)
    {
        var response = await _flurlClient
            .Request("depth")
            .SetQueryParam("symbol", symbol.ToUpperInvariant())
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