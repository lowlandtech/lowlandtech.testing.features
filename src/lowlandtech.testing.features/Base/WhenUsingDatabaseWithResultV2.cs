namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Base class for database tests with result capture and proper lifecycle management.
/// </summary>
/// <remarks>
/// This is an improved version of <see cref="WhenUsingDatabaseWithResult{TContext, TResult}"/> with:
/// - Proper IAsyncLifetime (no blocking)
/// - CancellationToken support
/// - Automatic database lifecycle management
/// - Consistent naming
/// </remarks>
/// <example>
/// public sealed class WhenSeedingSuperadmin : WhenUsingDatabaseWithResultV2&lt;GraphContext, SeededAdmin&gt;
/// {
///     protected override async Task GivenAsync(CancellationToken ct)
///     {
///         await Db.Use&lt;AdminAgentUseCase&gt;(ct);
///     }
///
///     protected override async Task&lt;SeededAdmin&gt; WhenWithResultAsync(CancellationToken ct)
///     {
///         var user = await Db.Nodes.FindAsync(new[] { AdminAgentUseCase.UserId }, ct);
///         var agent = await Db.Nodes.FindAsync(new[] { AdminAgentUseCase.AgentId }, ct);
///         var edge = Db.Edges.FirstOrDefault(e =>
///             e.SourceId == user!.Id &amp;&amp;
///             e.TargetId == agent!.Id &amp;&amp;
///             e.Label == "has.agent");
///
///         return new SeededAdmin(user, agent, edge);
///     }
///
///     [Fact]
///     [Then("User is seeded", "VCHIP-4001-UAC001")]
///     public void UserIsSeeded() => Result.User.ShouldNotBeNull();
///
///     [Fact]
///     [Then("Agent is seeded", "VCHIP-4001-UAC002")]
///     public void AgentIsSeeded() => Result.Agent.ShouldNotBeNull();
///
///     [Fact]
///     [Then("Edge links user and agent", "VCHIP-4001-UAC003")]
///     public void EdgeLinksUserAndAgent() => Result.Edge.ShouldNotBeNull();
/// }
///
/// public sealed record SeededAdmin(Node? User, Node? Agent, Edge? Edge);
/// </example>
/// <typeparam name="TContext">The type of the database context.</typeparam>
/// <typeparam name="TResult">The type of the result captured from the When operation.</typeparam>
public abstract class WhenUsingDatabaseWithResultV2<TContext, TResult> : WhenUsingDatabaseV2<TContext>
    where TContext : DbContext
{
    /// <summary>
    /// Gets the result captured from the When operation.
    /// </summary>
    protected TResult Result { get; private set; } = default!;

    /// <summary>
    /// Executes the operation and returns a result.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task containing the result of the operation.</returns>
    protected abstract Task<TResult> WhenWithResultAsync(CancellationToken ct);

    /// <inheritdoc/>
    protected sealed override async Task WhenAsync(CancellationToken ct)
    {
        Result = await WhenWithResultAsync(ct).ConfigureAwait(false);
    }
}
