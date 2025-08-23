namespace LowlandTech.Testing.Features.Reporter.Infrastructure.Models;

/// <summary>
/// Represents a branch in a command-line interface (CLI) hierarchy, containing a name and a collection of child
/// commands.
/// </summary>
/// <param name="Name">The name of the branch, which identifies it within the CLI hierarchy. Cannot be null or empty.</param>
/// <param name="Children">An array of child commands associated with this branch. Can be empty but not null.</param>
public sealed record CliBranch(string Name, params CliCmd[] Children);