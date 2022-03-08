using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BacktestUnicorn.Mediator;
using ICommand = BacktestUnicorn.Mediator.ICommand;

BenchmarkRunner.Run<MediatorBenchmark>();

[MemoryDiagnoser]
public class MediatorBenchmark
{
    private IServiceProvider _serviceProvider;

    [GlobalSetup]
    public void Setup()
    {
        _serviceProvider = new ServiceCollection()
            .AddMediatR(typeof(MediatorBenchmark))
            .AddMediator(typeof(MediatorBenchmark).Assembly)
            .AddTransient<ResponseService>()
            .BuildServiceProvider();
    }

    [Benchmark(Description = "MediatR command")]
    public async Task MediatrCommand()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var _ = await mediator.Send(new TestCommand());
    }
    
    [Benchmark(Description = "MediatR query")]
    public async Task MediatrQuery()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var _ = await mediator.Send(new TestQuery());
    }

    [Benchmark(Description = "MediatR stream")]
    public async Task MediatrStream()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var stream = mediator.CreateStream(new TestStreamQuery());
        var objResult = new List<TestResponse>();
        await using var enumerator = stream.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
            objResult.Add(enumerator.Current);
    }
    
    [Benchmark(Description = "Unicorn Mediator command")]
    public async Task UnicornMediatrCommand()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<BacktestUnicorn.Mediator.IMediator>();
        await mediator.SendCommand(new TestCommand(), CancellationToken.None);
    }
    
    [Benchmark(Description = "Unicorn Mediator query")]
    public async Task UnicornMediatrQuery()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<BacktestUnicorn.Mediator.IMediator>();
        var _ = await mediator.Query<TestQuery, TestResponse>(new TestQuery(), CancellationToken.None);
    }

    [Benchmark(Description = "Unicorn Mediator stream")]
    public async Task UnicornMediatrStream()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<BacktestUnicorn.Mediator.IMediator>();
        var stream = mediator.QueryStream<TestStreamQuery, TestResponse>(new TestStreamQuery(), CancellationToken.None);
        var objResult = new List<TestResponse>();
        await using var enumerator = stream.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
            objResult.Add(enumerator.Current);
    }
    
    [Benchmark(Description = "Direct command")]
    public async Task DirectCommand()
    {
        var responseService = new ResponseService();
        IRequestHandler<TestCommand> handler = new TestCommandHandler(responseService);
        var _ = await handler.Handle(new TestCommand(), CancellationToken.None);
    }
    
    [Benchmark(Description = "Direct query")]
    public async Task DirectQuery()
    {
        var responseService = new ResponseService();
        var handler = new TestQueryHandler(responseService);
        var _ = await handler.Handle(new TestQuery(), CancellationToken.None);
    }

    [Benchmark(Description = "Direct stream")]
    public async Task DirectStream()
    {
        var responseService = new ResponseService();
        var handler = new TestStreamHandler(responseService);
        var stream = handler.Handle(new TestStreamQuery(), CancellationToken.None);
        var objResult = new List<TestResponse>();
        await using var enumerator = stream.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
            objResult.Add(enumerator.Current);
    }
}

public record TestResponse(string Name = "Test Name");
public record TestCommand : ICommand, IRequest;
public class TestCommandHandler : ICommandHandler<TestCommand>, IRequestHandler<TestCommand>
{
    private readonly ResponseService _responseService;

    public TestCommandHandler(ResponseService responseService)
    {
        _responseService = responseService;
    }

    public Task Handle(TestCommand command, CancellationToken cancellationToken)
    {
        _responseService.Create();
        return Task.CompletedTask;
    }

    Task<Unit> IRequestHandler<TestCommand, Unit>.Handle(TestCommand request, CancellationToken cancellationToken)
    {
        _responseService.Create();
        return Unit.Task;
    }
}
public record TestQuery : IQuery<TestResponse>, IRequest<TestResponse>;
public class TestQueryHandler : IQueryHandler<TestQuery, TestResponse>, IRequestHandler<TestQuery, TestResponse>
{
    private readonly ResponseService _responseService;

    public TestQueryHandler(ResponseService responseService)
    {
        _responseService = responseService;
    }

    public Task<TestResponse> Handle(TestQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(_responseService.Create());
    }
}
public record TestStreamQuery : IStreamQuery<TestResponse>, IStreamRequest<TestResponse>;

public class TestStreamHandler : IStreamQueryHandler<TestStreamQuery, TestResponse>, IStreamRequestHandler<TestStreamQuery, TestResponse>
{
    private readonly ResponseService _responseService;

    public TestStreamHandler(ResponseService responseService)
    {
        _responseService = responseService;
    }

    public async IAsyncEnumerable<TestResponse> Handle(TestStreamQuery query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var testResponse in Enumerable.Range(0, 10).Select(x => $"Test Name {x:0000}"))
        {
            yield return await Task.FromResult(_responseService.Create(testResponse));
        }
    }
}
public class ResponseService
{
    public TestResponse Create(string name = "Test Name")
    {
        return new TestResponse(name);
    }
}
