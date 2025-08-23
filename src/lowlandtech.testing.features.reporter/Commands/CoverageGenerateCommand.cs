namespace LowlandTech.Testing.Features.Reporter.Commands;

/// <summary>
/// Represents the settings used to configure the generation of a coverage report.
/// </summary>
/// <remarks>This class encapsulates the command-line options required to generate a coverage report,  including
/// the path to the test assembly, the output file path, and an optional report title.</remarks>
public sealed class CoverageGenerateSettings : CommandSettings
{
    /// <summary>
    /// Gets the path to the test assembly file.
    /// </summary>
    /// <remarks>This property is initialized via the <c>--assembly</c> command-line option. Ensure the
    /// specified path points to a valid test assembly file.</remarks>
    [CommandOption("--assembly <PATH>")]
    [Description("Path to test assembly (.dll)")]
    public string AssemblyPath { get; init; } = default!;

    /// <summary>
    /// Gets the file path where the output markdown file will be saved.
    /// </summary>
    [CommandOption("--out <PATH>")]
    [Description("Output markdown file")]
    public string OutputPath { get; init; } = default!;

    /// <summary>
    /// Gets the title of the report.
    /// </summary>
    [CommandOption("--title <TEXT>")]
    [Description("Report title")]
    public string? Title { get; init; }
}

/// <summary>
/// Executes the command to generate a code coverage report for the specified assembly.
/// </summary>
/// <remarks>This command generates a code coverage report for the specified assembly and saves it to the
/// specified output path. The output directory is created if it does not already exist. The report includes the title
/// specified in the settings.</remarks>
public sealed class CoverageGenerateCommand : Command<CoverageGenerateSettings>
{
    /// <summary>
    /// Executes the command to generate a coverage report for the specified assembly.
    /// </summary>
    /// <remarks>This method creates the output directory if it does not already exist, loads the specified
    /// assembly, and generates a coverage report at the specified output path. A success message is displayed upon
    /// completion.</remarks>
    /// <param name="ctx">The context in which the command is executed.</param>
    /// <param name="s">The settings used to configure the coverage report generation, including the assembly path, output path, and
    /// report title.</param>
    /// <returns>An integer indicating the result of the command execution. Returns 0 if the operation completes successfully.</returns>
    public override int Execute(CommandContext ctx, CoverageGenerateSettings s)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(s.OutputPath)!);
        var asm = Assembly.LoadFrom(s.AssemblyPath);

        // Reuse your extension (sync is fine here)
        asm.GenerateCoverageReport(s.OutputPath, s.Title); // :contentReference[oaicite:0]{index=0}

        AnsiConsole.MarkupLine($"[green]✅ Coverage report generated:[/] {s.OutputPath}");
        return 0;
    }
}
