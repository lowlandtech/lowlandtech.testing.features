namespace LowlandTech.Testing.Features.Reporter.Commands;

public sealed class CoverageGenerateSettings : CommandSettings
{
    [CommandOption("--assembly <PATH>")]
    [Description("Path to test assembly (.dll)")]
    public string AssemblyPath { get; init; } = default!;

    [CommandOption("--out <PATH>")]
    [Description("Output markdown file")]
    public string OutputPath { get; init; } = default!;

    [CommandOption("--title <TEXT>")]
    [Description("Report title")]
    public string? Title { get; init; }
}

public sealed class CoverageGenerateCommand : Command<CoverageGenerateSettings>
{
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
