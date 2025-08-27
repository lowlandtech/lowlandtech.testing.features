using LowlandTech.Scripts.Abstractions.Models;

namespace LowlandTech.Testing.Features.Reporter.Infrastructure.Sinks;

/// <summary>
/// Provides functionality to persist scenario results to disk.
/// </summary>
/// <remarks>This class implements the <see cref="IResultSink"/> interface, enabling the storage of  scenario
/// results in a specified output directory. It supports configuration of the  output directory and operations to upsert
/// results asynchronously.</remarks>
public sealed class DiskSink : IResultSink
{
    /// <inheritdoc />
    public string Name => "disk";
    private string _outDir = ".";

    /// <summary>
    /// Configures the output directory for the disk sink.
    /// </summary>
    /// <param name="outDir">The path to the output directory. If <see langword="null"/> or whitespace, the current output directory remains
    /// unchanged.</param>
    /// <returns>The current <see cref="DiskSink"/> instance, allowing for method chaining.</returns>
    public DiskSink Configure(string? outDir = null)
    {
        if (!string.IsNullOrWhiteSpace(outDir)) _outDir = outDir!;
        return this;
    }

    /// <inheritdoc />
    public bool Supports(string target) => target.Equals("disk", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Task UpsertAsync(ScenarioResultEnvelope e, CancellationToken ct = default)
    {
        var vchip = e.ScenarioId.Split('-')[1].ToLowerInvariant();
        var dir = Path.Combine(_outDir, $"vchip-{vchip}");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{e.ScenarioId}.md");

        var content = $$"""
                        ---
                        scenario_id: {{e.ScenarioId}}
                        status: {{e.Status}}
                        commit: {{e.Commit ?? ""}}
                        run_id: {{e.RunId ?? ""}}
                        natural_key: {{e.NaturalKey}}
                        started_at: {{e.StartedAt:O}}
                        finished_at: {{e.FinishedAt:O}}
                        duration_ms: {{e.DurationMs}}
                        ---

                        # {{e.ScenarioId}}

                        Status: **{{e.Status}}**
                        """;

        File.WriteAllText(path, content);
        return Task.CompletedTask;
    }
}
