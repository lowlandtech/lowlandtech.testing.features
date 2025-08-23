namespace LowlandTech.Testing.Features.Reporter.Commands;

/// <summary>
/// Represents the settings for reporting a scenario's results, including options for scenario identification, status,
/// output targets, and additional metadata.
/// </summary>
/// <remarks>This class is used to configure the parameters for a command that generates a report for a specific
/// scenario.  It includes options for specifying the scenario ID, status, output targets, and optional metadata such as
/// commit SHA or run ID.</remarks>
public sealed class ReportScenarioSettings : CommandSettings
{
    /// <summary>
    /// Gets the unique identifier for the scenario.
    /// </summary>
    [CommandOption("--id <SCENARIO_ID>")] public string ScenarioId { get; init; } = default!;

    /// <summary>
    /// Gets the status of the command execution.
    /// </summary>
    [CommandOption("--status <STATUS>")] public string Status { get; init; } = "passed";

    /// <summary>
    /// Gets the target destination(s) for the operation.
    /// </summary>
    /// <remarks>This property is initialized via the <c>--to</c> command-line option.  Specify the target(s)
    /// as a string, such as "disk" or other valid destinations.</remarks>
    [CommandOption("--to <TARGETS>")] public string Targets { get; init; } = "disk";

    /// <summary>
    /// Gets the output directory where the generated files will be saved.
    /// </summary>
    /// <remarks>The directory must be writable. Ensure the specified path exists or can be created by the
    /// application.</remarks>
    [CommandOption("--out <DIR>")] public string? OutDir { get; init; } // disk only

    /// <summary>
    /// Gets the commit SHA to be used for the operation.
    /// </summary>
    [CommandOption("--commit <SHA>")] public string? Commit { get; init; }

    /// <summary>
    /// Gets the unique identifier for the current run of the command.
    /// </summary>
    [CommandOption("--run-id <ID>")] public string? RunId { get; init; }
}

/// <summary>
/// Represents a command that reports the results of a scenario to one or more result sinks.
/// </summary>
/// <remarks>This command processes the specified scenario results and sends them to the appropriate result sinks
/// based on the provided targets. If a target is not supported by any available sink, an exception is thrown.</remarks>
public sealed class ReportScenarioCommand(IEnumerable<IResultSink> sinks) : Command<ReportScenarioSettings>
{
    /// <summary>
    /// Executes the reporting process for a given scenario, sending the results to the specified targets.
    /// </summary>
    /// <remarks>This method processes the scenario results by creating a result envelope and sending it to
    /// the configured targets. If a target is associated with a disk sink and an output directory is specified in
    /// <paramref name="s"/>, the disk sink is configured to use the specified directory. The method blocks until all
    /// targets have been processed.</remarks>
    /// <param name="ctx">The command context in which the execution is performed. This parameter provides contextual information about
    /// the command being executed.</param>
    /// <param name="s">The settings for the scenario report, including the scenario ID, status, commit, run ID, output directory, and
    /// targets.</param>
    /// <returns>An integer indicating the result of the execution. Returns <see langword="0"/> to indicate successful execution.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a specified target in <paramref name="s"/> is not supported by any available sink.</exception>
    public override int Execute(CommandContext ctx, ReportScenarioSettings s)
    {
        var envelope = ScenarioResultEnvelope.ForScenario(
            s.ScenarioId, s.Status, s.Commit, s.RunId);

        foreach (var target in SplitTargets(s.Targets))
        {
            var sink = sinks.FirstOrDefault(x => x.Supports(target));
            if (sink == null) throw new InvalidOperationException($"Unknown target '{target}'");

            if (sink is DiskSink disk && s.OutDir is not null)
                disk.Configure(outDir: s.OutDir);

            sink.UpsertAsync(envelope).GetAwaiter().GetResult();
        }

        AnsiConsole.MarkupLine($"[green]Reported[/] {s.ScenarioId} to {s.Targets}");
        return 0;
    }

    /// <summary>
    /// Splits a comma-separated string into a collection of individual, trimmed, non-empty substrings.
    /// </summary>
    /// <param name="v">The input string containing comma-separated values.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of strings, where each string is a trimmed, non-empty substring  from the input
    /// string. Returns an empty collection if the input string is null or contains no valid entries.</returns>
    private static IEnumerable<string> SplitTargets(string v)
        => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
