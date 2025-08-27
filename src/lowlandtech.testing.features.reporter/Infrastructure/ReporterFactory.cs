namespace LowlandTech.Testing.Features.Reporter.Infrastructure;

/// <summary>
/// Provides methods to configure and build a <see cref="CommandApp"/> instance with predefined commands and services.
/// </summary>
/// <remarks>This factory class simplifies the creation and configuration of a <see cref="CommandApp"/> by setting
/// up application commands, branches, and dependencies. It includes predefined branches for "coverage" and "report"
/// commands, each with specific subcommands.</remarks>
public class ReporterFactory
{
    /// <summary>
    /// Configures the application by setting its name and defining available branches and commands.
    /// </summary>
    /// <remarks>This method sets the application name to "ltr" and defines two branches:  <list
    /// type="bullet"> <item> <description> <c>coverage</c>: Contains the <c>generate</c> command, which generates a
    /// coverage markdown from a test assembly. </description> </item> <item> <description> <c>report</c>: Contains two
    /// commands: <list type="bullet"> <item><c>scenario</c>: Reports a single scenario result to one or more
    /// targets.</item> <item><c>run</c>: Reports a full test run (e.g., TRX) to one or more targets.</item> </list>
    /// </description> </item> </list></remarks>
    /// <param name="cfg">The configurator used to define application settings, branches, and commands.</param>
    public static void Configure(IConfigurator cfg)
    {
        cfg.SetApplicationName("ltr");
        cfg.PropagateExceptions();

        foreach (var b in ReporterCatalog.Roots)
        {
            cfg.AddBranch(b.Name, bb =>
            {
                foreach (var cmd in b.Children)
                    bb.AddCommand(cmd.Name, cmd.CommandType);
            });
        }
    }

    /// <summary>
    /// Creates and configures a new instance of <see cref="CommandApp"/> with optional dependency injection support.
    /// </summary>
    /// <remarks>This method sets up logging and dependency injection for the application. It uses a custom
    /// <see cref="TypeRegistrar"/> to integrate the provided or newly created service collection.</remarks>
    /// <param name="services">An optional <see cref="IServiceCollection"/> to register services for dependency injection. If <paramref
    /// name="services"/> is <see langword="null"/>, a new <see cref="ServiceCollection"/> is created.</param>
    /// <returns>A fully configured <see cref="CommandApp"/> instance ready for use.</returns>
    public static CommandApp Build(IServiceCollection? services = null)
    {
        services = services ?? new ServiceCollection();
        services.AddLogging();
        // Register sinks, commands’ deps, etc. (You can add real ones later)
        services.AddSingleton<IResultSink, DiskSink>();

        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);
        app.Configure(Configure);
        registrar.Build();
        return app;
    }
}
