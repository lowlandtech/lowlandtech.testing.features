namespace LowlandTech.Testing.Features.Reporter.Commands;

public sealed class ReportRunSettings : CommandSettings
{
    [CommandOption("--from <PATH>")] public string From { get; init; } = default!;
    [CommandOption("--to <TARGETS>")] public string Targets { get; init; } = "disk";
    [CommandOption("--out <DIR>")] public string? OutDir { get; init; } // disk only
    [CommandOption("--commit <SHA>")] public string? Commit { get; init; }
    [CommandOption("--run-id <ID>")] public string? RunId { get; init; }
}

public sealed class ReportRunCommand : Command<ReportRunSettings>
{
    private readonly IEnumerable<IResultSink> _sinks;
    public ReportRunCommand(IEnumerable<IResultSink> sinks) => _sinks = sinks;

    public override int Execute(CommandContext ctx, ReportRunSettings s)
    {
        // TODO: Parse TRX/JUnit/XUnit XML -> IEnumerable<ScenarioResultEnvelope>
        var envelopes = Enumerable.Empty<ScenarioResultEnvelope>();

        foreach (var env in envelopes)
        {
            foreach (var target in s.Targets.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var sink = _sinks.FirstOrDefault(x => x.Supports(target))
                           ?? throw new InvalidOperationException($"Unknown target '{target}'");

                if (sink is DiskSink disk && s.OutDir is not null)
                    disk.Configure(outDir: s.OutDir);

                sink.UpsertAsync(env).GetAwaiter().GetResult();
            }
        }

        AnsiConsole.MarkupLine($"[yellow]No run parser implemented yet[/]");
        return 0;
    }
}