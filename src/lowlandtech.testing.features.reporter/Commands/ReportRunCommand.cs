using LowlandTech.Scripts.Abstractions.Models;

namespace LowlandTech.Testing.Features.Reporter.Commands;

/// <summary>
/// Represents the settings used to configure the execution of a report command.
/// </summary>
/// <remarks>This class provides options for specifying the source, target, output directory, and other parameters
/// required to run a report. It is designed to be used with command-line arguments.</remarks>
public sealed class ReportRunSettings : CommandSettings
{
    /// <summary>
    /// Gets the source file or directory path specified by the <c>--from</c> command-line option.
    /// </summary>
    [CommandOption("--from <PATH>")] public string From { get; init; } = default!;

    /// <summary>
    /// Gets the target destination(s) for the operation.
    /// </summary>
    /// <remarks>This property is initialized via the <c>--to</c> command-line option.  Specify the target(s)
    /// as a string, such as "disk" or other valid destinations.</remarks>
    [CommandOption("--to <TARGETS>")] public string Targets { get; init; } = "disk";

    /// <summary>
    /// Gets the output directory where the generated files will be saved.
    /// </summary>
    /// <remarks>The directory path must be valid and writable. Ensure the specified directory exists or can
    /// be created before using this property.</remarks>
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
/// Executes the report generation process by processing test result data and sending it to the configured result sinks.
/// </summary>
/// <remarks>This command processes test result data (e.g., TRX, JUnit, or XUnit XML formats) and distributes the
/// results to the specified sinks. The sinks are determined by the <see cref="ReportRunSettings.Targets"/> property,
/// and each sink must support the specified target. If a <see cref="DiskSink"/> is used and an output directory is
/// provided, the sink is configured to write results to the specified directory.</remarks>
/// <param name="sinks"></param>
public sealed class ReportRunCommand(IEnumerable<IResultSink> sinks) : Command<ReportRunSettings>
{
    /// <summary>
    /// Executes the command to process test results and send them to the specified targets.
    /// </summary>
    /// <remarks>This method processes test result data and sends it to the configured targets specified in
    /// the <paramref name="s"/> parameter. Each target must be supported by a corresponding sink. If a target is not
    /// recognized, an exception is thrown.  The method currently does not implement parsing of test result files (e.g.,
    /// TRX, JUnit, or XUnit formats).</remarks>
    /// <param name="ctx">The context of the command execution, providing necessary runtime information.</param>
    /// <param name="s">The settings for the report run, including target destinations and output directory.</param>
    /// <returns>An integer indicating the result of the execution. Returns 0 if the operation completes successfully.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a specified target in <paramref name="s.Targets"/> is not supported by any available sink.</exception>
    public override int Execute(CommandContext ctx, ReportRunSettings s)
    {
        // TODO: Parse TRX/JUnit/XUnit XML -> IEnumerable<ScenarioResultEnvelope>
        var envelopes = Enumerable.Empty<ScenarioResultEnvelope>();

        foreach (var env in envelopes)
        {
            foreach (var target in s.Targets.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var sink = sinks.FirstOrDefault(x => x.Supports(target))
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