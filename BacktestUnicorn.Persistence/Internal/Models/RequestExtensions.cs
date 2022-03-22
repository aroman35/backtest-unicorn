﻿using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Abstractions.StateManagement;
using MongoDB.Driver;

namespace BacktestUnicorn.Persistence.Internal.Models;

public static class RequestExtensions
{
    public static FilterDefinition<PersistenceModel<TState>> Filter<TState>(this UpdateStateCommand<TState> command)
        where TState : State
    {
        var filter = Builders<PersistenceModel<TState>>.Filter.Eq(x => x.Id, command.Id);
        return filter;
    }
}