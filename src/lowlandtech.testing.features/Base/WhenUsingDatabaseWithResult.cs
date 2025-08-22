namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Same GWT pattern, but When returns a typed result that the base captures in Result.
/// No Act method, no change to For semantics.
/// </summary>
/// <example>
/// public sealed class WhenSeedingSuperadmin_WithResult
///     : WhenUsingDatabaseWithResult<GraphContext, SeededAdmin>
/// {
///     protected override async Task GivenAsync()
///     {
///         await Db.Use<AdminAgentUseCase>();
///     }
///
///     protected override async Task<SeededAdmin> WhenAsyncWithResult()
///     {
///         var user  = await Db.Nodes.FindAsync(AdminAgentUseCase.UserId);
///         var agent = await Db.Nodes.FindAsync(AdminAgentUseCase.AgentId);
///         var edge  = Db.Edges.FirstOrDefault(e =>
///             e.SourceId == user!.Id && e.TargetId == agent!.Id && e.Label == "has.agent");
///
///         return new SeededAdmin(user, agent, edge);
///     }
///
///     [Fact]
///     public void UserIsSeeded() => Result.User.ShouldNotBeNull();
///
///     [Fact]
///     public void AgentIsSeeded() => Result.Agent.ShouldNotBeNull();
///
///     [Fact]
///     public void EdgeLinksUserAndAgent() => Result.Edge.ShouldNotBeNull();
///
///     [Fact]
///     public void UserHasAdminRole()
///         => Result.User!.Properties["Role"].ShouldBe("admin");
///
///     [Fact]
///     public void UserHasKeys()
///     {
///         Result.User!.Properties["Mnemonic"].ShouldNotBeNullOrWhiteSpace();
///         Result.User!.Properties["PublicKey"].ShouldNotBeNullOrWhiteSpace();
///     }
/// }
///
/// public sealed record SeededAdmin(Node? User, Node? Agent, Edge? Edge);
/// </example>
public abstract class WhenUsingDatabaseWithResult<TContext, TResult>
    : WhenUsingDatabase<TContext> where TContext : DbContext
{
    protected TResult Result { get; private set; } = default!;

    // Your GivenAsync stays as-is.
    protected abstract Task<TResult> WhenAsyncWithResult();

    // The original WhenAsync is sealed here and just captures the result.
    protected sealed override async Task WhenAsync()
    {
        Result = await WhenAsyncWithResult();
    }
}
