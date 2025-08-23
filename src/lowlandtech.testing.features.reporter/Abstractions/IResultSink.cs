namespace LowlandTech.Testing.Features.Reporter.Abstractions;

/// <summary>
/// Defines a sink for processing and storing scenario results.
/// </summary>
/// <remarks>Implementations of this interface are responsible for handling the storage or processing of scenario
/// results, as well as determining compatibility with specific targets.</remarks>
public interface IResultSink
{
    /// <summary>
    /// Gets the name associated with the current instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Inserts a new scenario result or updates an existing one asynchronously.
    /// </summary>
    /// <remarks>This method ensures that the provided scenario result is either added to the data store if it
    /// does not already exist,  or updated if it does. The operation is performed asynchronously to avoid blocking the
    /// calling thread.</remarks>
    /// <param name="env">The <see cref="ScenarioResultEnvelope"/> object containing the scenario result data to be inserted or updated. 
    /// This parameter cannot be <see langword="null"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the scenario result has been
    /// successfully inserted or updated.</returns>
    Task UpsertAsync(ScenarioResultEnvelope env, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified target is supported.
    /// </summary>
    /// <param name="target">The target to check for support. This value cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the specified target is supported; otherwise, <see langword="false"/>.</returns>
    bool Supports(string target);
}
