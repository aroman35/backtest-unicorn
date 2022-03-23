using AutoMapper;
using Hermes.MarketData.Abstractions.OrderBook;

namespace Hermes.Connectors.MarketData.ExcahngeModels.Binance;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<OrderBookSnapshotModel, OrderBookSnapshot>();
        CreateMap<OrderBookDiffExchangeModel, OrderBookDiffModel>();
    }
}