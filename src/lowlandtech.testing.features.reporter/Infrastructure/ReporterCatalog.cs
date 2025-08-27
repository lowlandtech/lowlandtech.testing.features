using LowlandTech.Scripts.Abstractions.Models;

namespace LowlandTech.Testing.Features.Reporter.Infrastructure;

/// <summary>
/// Provides a catalog of CLI (Command-Line Interface) branches and commands for organizing and accessing the available
/// CLI functionality.
/// </summary>
/// <remarks>The <see cref="ReporterCatalog"/> class defines a set of predefined CLI branches, each containing specific
/// commands. These branches represent the root-level structure of the CLI and can be used to navigate or execute
/// commands.</remarks>
public static class ReporterCatalog
{

    /// <summary>
    /// Represents the "coverage" branch of CLI commands, which includes subcommands related to code coverage
    /// operations.
    /// </summary>
    /// <remarks>This branch is used to group commands related to code coverage functionality.  For example,
    /// it includes the "generate" command, which is associated with generating coverage reports.</remarks>
    public static readonly CliBranch Coverage = new("coverage",
        new CliCmd("generate", typeof(CoverageGenerateCommand)));

    /// <summary>
    /// Represents the "report" branch of the CLI, containing commands related to reporting functionality.
    /// </summary>
    /// <remarks>This branch includes the following commands: <list type="bullet"> <item>
    /// <description><c>scenario</c>: Executes the <see cref="ReportScenarioCommand"/> to handle scenario-based
    /// reporting.</description> </item> <item> <description><c>run</c>: Executes the <see cref="ReportRunCommand"/> to
    /// handle report execution.</description> </item> </list></remarks>
    public static readonly CliBranch Report = new("report",
        new CliCmd("scenario", typeof(ReportScenarioCommand)),
        new CliCmd("run", typeof(ReportRunCommand)));

    /// <summary>
    /// Gets the root branches of the CLI command hierarchy.
    /// </summary>
    public static IEnumerable<CliBranch> Roots => [Coverage, Report];
}