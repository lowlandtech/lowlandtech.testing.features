namespace LowlandTech.Testing.Features.Reporter.Infrastructure;

/// <summary>
/// Provides extension methods for adding commands to a configurator in a dynamic manner.
/// </summary>
/// <remarks>These methods allow commands to be added to a configurator by specifying the command name and type.
/// The configurator's safety checks are temporarily disabled during the addition process.</remarks>
public static class SpectreDynamic
{
    /// <summary>
    /// Adds a command to the configurator with the specified name and type.
    /// </summary>
    /// <param name="config">The configurator to which the command will be added.</param>
    /// <param name="name">The name of the command. This must be unique within the configurator.</param>
    /// <param name="commandType">The type of the command to add. This must be a valid command type.</param>
    public static void AddCommand(this IConfigurator config, string name, Type commandType) =>
        config.SafetyOff().AddCommand(name, commandType);

    /// <summary>
    /// Adds a command to the configurator with the specified name and type.
    /// </summary>
    /// <remarks>This method extends the configurator to allow adding commands dynamically. The command type
    /// must be compatible with the  specified settings type <typeparamref name="TSettings"/>.</remarks>
    /// <typeparam name="TSettings">The type of settings associated with the command. Must inherit from <see cref="CommandSettings"/>.</typeparam>
    /// <param name="config">The configurator to which the command will be added.</param>
    /// <param name="name">The name of the command to add. This name is used to identify the command.</param>
    /// <param name="commandType">The type of the command to add. Must be a valid type that implements the command logic.</param>
    public static void AddCommand<TSettings>(this IConfigurator<TSettings> config, string name, Type commandType) where TSettings : CommandSettings =>
        config.SafetyOff().AddCommand(name, commandType);
}