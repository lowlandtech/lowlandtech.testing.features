namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Represents an asynchronous test scenario where a result of type <typeparamref name="TResult"/> is produced by the
/// operation being tested.
/// </summary>
/// <remarks>This abstract class provides a framework for testing asynchronous operations that produce a result.
/// Derived classes must implement the <see cref="WhenWithResultAsync"/> method to define the specific operation being
/// tested. The result of the operation is stored in the <see cref="Result"/> property after the operation
/// completes.</remarks>
/// <typeparam name="TSut">The type of the system under test (SUT).</typeparam>
/// <typeparam name="TResult">The type of the result produced by the asynchronous operation.</typeparam>
/// <example>
/// public sealed class WhenReadingSuperadminProjectionAsync
///     : WhenTestingForWithResultAsync<AdminAgentReader, SeededAdmin>
/// {
///     private IFakeGraphReadStore _store = default!;
///
///     protected override AdminAgentReader For()
///         => new AdminAgentReader(_store); // SUT depends on a store/repo, not the DB base
///
///     protected override async Task GivenAsync()
///     {
///         // Seed the fake read-store the way AdminAgentUseCase would have done
///         _store = new FakeGraphReadStore();
///         await _store.SeedAdminAsync(
///             AdminAgentUseCase.UserId,
///             AdminAgentUseCase.AgentId,
///             edgeLabel: "has.agent");
///     }
///
///     protected override Task<SeededAdmin> WhenWithResultAsync()
///         => Sut.ReadAsync(); // returns SeededAdmin(User, Agent, Edge)
///
///     [Fact]
///     public void UserIsSeeded() => Result.User.ShouldNotBeNull();
///
///     [Fact]
///     public void AgentIsSeeded() => Result.Agent.ShouldNotBeNull();
///
///     [Fact]
///     public void EdgeLinksUserAndAgent() => Result.Edge.ShouldNotBeNull();
/// }
///
/// // Example SUT & result types used in the doc:
/// public sealed class AdminAgentReader(IFakeGraphReadStore store)
/// {
///     public async Task<SeededAdmin> ReadAsync()
///     {
///         var user  = await store.FindNodeAsync(AdminAgentUseCase.UserId);
///         var agent = await store.FindNodeAsync(AdminAgentUseCase.AgentId);
///         var edge  = store.FindEdge(user!.Id, agent!.Id, "has.agent");
///         return new SeededAdmin(user, agent, edge);
///     }
/// }
///
/// public interface IFakeGraphReadStore
/// {
///     Task<Node?> FindNodeAsync(Guid id);
///     Edge? FindEdge(Guid sourceId, Guid targetId, string label);
///     Task SeedAdminAsync(Guid userId, Guid agentId, string edgeLabel);
/// }
///
/// public sealed record SeededAdmin(Node? User, Node? Agent, Edge? Edge);
/// </example>

public abstract class WhenTestingForWithResultAsync<TSut, TResult> : WhenTestingForAsync<TSut>
{
    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    protected TResult Result { get; private set; } = default!;

    /// <summary>
    /// Executes an asynchronous operation and returns a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define the specific
    /// asynchronous operation that produces a result. The implementation should ensure that the returned task completes
    /// successfully or with an appropriate exception if an error occurs.</remarks>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task's result contains the value of
    /// type <typeparamref name="TResult"/> produced by the operation.</returns>
    protected abstract Task<TResult> WhenWithResultAsync();

    /// <inheritdoc/>
    protected sealed override async Task WhenAsync() => Result = await WhenWithResultAsync();
}