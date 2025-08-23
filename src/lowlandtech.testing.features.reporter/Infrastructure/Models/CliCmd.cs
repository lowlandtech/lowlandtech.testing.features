namespace LowlandTech.Testing.Features.Reporter.Infrastructure.Models;

/// <summary>
/// Represents a command in a command-line interface (CLI) application,  including its name and associated type.
/// </summary>
/// <param name="Name">The name of the command as it appears in the CLI.</param>
/// <param name="CommandType">The <see cref="Type"/> that implements the logic for the command.  This type must be a class that defines the
/// behavior of the command.</param>
public sealed record CliCmd(string Name, Type CommandType);