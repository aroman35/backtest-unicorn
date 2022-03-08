using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BacktestUnicorn.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace BacktestUnicorn.MediatorTests;

public class MediatorTests
{
    private readonly IServiceProvider _serviceProvider;

    public MediatorTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddMediator(typeof(TestCommandHandler).Assembly)
            .BuildServiceProvider();
    }

    [Fact]
    public async Task CommandResultTest()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var commandResult = await mediator.SendCommand<TestCommandWithResponse, TestResponse>(new TestCommandWithResponse(), CancellationToken.None);
        commandResult.Name.ShouldBe("Test Name");
    }

    [Fact]
    public async Task QueryResultTest()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var queryResult = await mediator.Query<TestQuery, TestResponse>(new TestQuery(), CancellationToken.None);
        queryResult.Name.ShouldBe("Test Name");
    }

    [Fact]
    public async Task StreamResultTest()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var streamResult = mediator
            .QueryStream<TestStreamQuery, TestResponse>(new TestStreamQuery(), CancellationToken.None);

        var counter = 0;
        await using var enumerator = streamResult.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
            enumerator.Current.Name.ShouldBe($"Test Name {counter++:0000}");
    }
}

public record TestResponse(string Name = "Test Name");
public record TestCommand : ICommand;
public class TestCommandHandler : ICommandHandler<TestCommand>
{
    public Task Handle(TestCommand command, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
public record TestCommandWithResponse : ICommand<TestResponse>;
public class TestCommandWithResponseHandler : ICommandHandler<TestCommandWithResponse, TestResponse>
{
    public Task<TestResponse> Handle(TestCommandWithResponse command, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse());
    }
}
public record TestQuery : IQuery<TestResponse>;
public class TestQueryHandler : IQueryHandler<TestQuery, TestResponse>
{
    public Task<TestResponse> Handle(TestQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse());
    }
}
public record TestStreamQuery : IStreamQuery<TestResponse>;

public class TestStreamHandler : IStreamQueryHandler<TestStreamQuery, TestResponse>
{
    public async IAsyncEnumerable<TestResponse> Handle(TestStreamQuery query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var testResponse in Enumerable.Range(0, 10).Select(x => new TestResponse($"Test Name {x:0000}")))
        {
            yield return await Task.FromResult(testResponse);
        }
    }
}