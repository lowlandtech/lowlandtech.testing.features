using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowlandTech.Testing.Features.Reporter.Commands;

using LowlandTech.Testing.Features.Reporter.Infrastructure.Models;

// Commands/ReportScenarioCommand.cs
using Spectre.Console.Cli;
using System.ComponentModel;

public sealed class ReportScenarioSettings : CommandSettings
{
    [CommandOption("--id <SCENARIO_ID>")] public string ScenarioId { get; init; } = default!;
    [CommandOption("--status <STATUS>")] public string Status { get; init; } = "passed";
    [CommandOption("--to <TARGETS>")] public string Targets { get; init; } = "disk";
    [CommandOption("--out <DIR>")] public string? OutDir { get; init; } // disk only
    [CommandOption("--commit <SHA>")] public string? Commit { get; init; }
    [CommandOption("--run-id <ID>")] public string? RunId { get; init; }
}

public sealed class ReportScenarioCommand : Command<ReportScenarioSettings>
{
    private readonly IEnumerable<IResultSink> _sinks;

    public ReportScenarioCommand(IEnumerable<IResultSink> sinks) => _sinks = sinks;

    public override int Execute(CommandContext ctx, ReportScenarioSettings s)
    {
        var envelope = ScenarioResultEnvelope.ForScenario(
            s.ScenarioId, s.Status, s.Commit, s.RunId);

        foreach (var target in SplitTargets(s.Targets))
        {
            var sink = _sinks.FirstOrDefault(x => x.Supports(target));
            if (sink == null) throw new InvalidOperationException($"Unknown target '{target}'");

            if (sink is DiskSink disk && s.OutDir is not null)
                disk.Configure(outDir: s.OutDir);

            sink.UpsertAsync(envelope).GetAwaiter().GetResult();
        }

        AnsiConsole.MarkupLine($"[green]Reported[/] {s.ScenarioId} to {s.Targets}");
        return 0;
    }

    private static IEnumerable<string> SplitTargets(string v)
        => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
