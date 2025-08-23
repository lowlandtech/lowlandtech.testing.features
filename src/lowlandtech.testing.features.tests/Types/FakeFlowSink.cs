using LowlandTech.Testing.Features.Reporter.Infrastructure.Models;

namespace LowlandTech.Testing.Features.Tests.Types;

public sealed class FakeFlowSink : IResultSink
{
    public string Name => "flow";
    public bool Supports(string target) => target.Equals("flow", StringComparison.OrdinalIgnoreCase);
    public readonly List<ScenarioResultEnvelope> Items = new();
    public Task UpsertAsync(ScenarioResultEnvelope e, CancellationToken ct = default)
    {
        Items.RemoveAll(x => x.NaturalKey == e.NaturalKey);
        Items.Add(e);
        return Task.CompletedTask;
    }
}
