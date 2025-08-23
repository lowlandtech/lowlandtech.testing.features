namespace LowlandTech.Testing.Features.Reporter;

/// <summary>
/// The entry point of the application.
/// </summary>
/// <remarks>This method initializes and configures the command-line application using the <see
/// cref="CommandApp"/> class. It defines two main branches of commands: <c>coverage</c> and <c>report</c>. <list
/// type="bullet"> <item> <description> The <c>coverage</c> branch includes a <c>generate</c> command for generating a
/// coverage markdown from a test assembly. </description> </item> <item> <description> The <c>report</c> branch
/// includes: <list type="bullet"> <item><c>scenario</c> command for reporting a single scenario result to one or more
/// targets.</item> <item><c>run</c> command for reporting a full test run (e.g., TRX) to one or more targets.</item>
/// </list> </description> </item> </list></remarks>
public static class Program
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    /// <remarks>If no command-line arguments are provided and the application is running in a debug build or
    /// under a debugger,  the application automatically displays help information by appending the <c>--help</c>
    /// argument.</remarks>
    /// <param name="args">An array of command-line arguments passed to the application.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the exit code of the application, 
    /// where 0 indicates success and non-zero values indicate errors or specific exit conditions.</returns>
    public static Task<int> Main(string[] args)
    {
        // If we're in a debug build (or a debugger is attached) AND no args were provided,
        // show help so devs see available commands immediately.
        if (ShouldShowHelpOnEmptyArgs(args))
        {
            args = ["--help"];
        }

        var app = ReporterFactory.Build();
        // Spectre's Run is sync; keep Main async-friendly by wrapping.
        var exit = app.Run(args);
        return Task.FromResult(exit);
    }

    /// <summary>
    /// Determines whether the help message should be displayed when no command-line arguments are provided.
    /// </summary>
    /// <remarks>In Debug builds, this method always returns <see langword="true"/> when no arguments are
    /// provided. In Release builds, it returns <see langword="true"/> only if no arguments are provided and a debugger
    /// is attached.</remarks>
    /// <param name="args">An array of command-line arguments. Can be <see langword="null"/> or empty.</param>
    /// <returns><see langword="true"/> if the help message should be displayed; otherwise, <see langword="false"/>.</returns>
    private static bool ShouldShowHelpOnEmptyArgs(string[]? args)
    {
        var noArgs = args is null || args.Length == 0;

#if DEBUG
        // In Debug builds: always treat empty args as --help
        return noArgs;
#else
        // In Release builds: only do this convenience if you're actively debugging
        return noArgs && Debugger.IsAttached;
#endif
    }
}