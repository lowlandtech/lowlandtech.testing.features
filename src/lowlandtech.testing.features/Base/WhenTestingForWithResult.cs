namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Represents a base class for testing scenarios where an operation produces a result of type <typeparamref
/// name="TResult"/>.
/// </summary>
/// <remarks>This class provides a framework for defining and executing test scenarios where the operation being
/// tested returns a result. Derived classes should implement the <see cref="WhenWithResult"/> method to define the
/// specific operation under test. The result of the operation is stored in the <see cref="Result"/> property for
/// further validation or assertions.</remarks>
/// <typeparam name="TSut">The type of the system under test (SUT).</typeparam>
/// <typeparam name="TResult">The type of the result produced by the operation.</typeparam>
/// <example>
/// public sealed class WhenReadingSuperadminProjection
///     : WhenTestingForWithResult<AdminAgentReaderSync, SeededAdmin>
/// {
///     private IFakeGraphReadStoreSync _store = default!;
///
///     protected override AdminAgentReaderSync For()
///         => new AdminAgentReaderSync(_store);
///
///     protected override Task GivenAsync()
///     {
///         _store = new FakeGraphReadStoreSync();
///         _store.SeedAdmin(
///             AdminAgentUseCase.UserId,
///             AdminAgentUseCase.AgentId,
///             edgeLabel: "has.agent");
///         return Task.CompletedTask;
///     }
///
///     protected override Task<SeededAdmin> WhenWithResult()
///         => Task.FromResult(Sut.Read()); // sync read, base captures Result
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
/// public sealed class AdminAgentReaderSync(IFakeGraphReadStoreSync store)
/// {
///     public SeededAdmin Read()
///     {
///         var user  = store.FindNode(AdminAgentUseCase.UserId);
///         var agent = store.FindNode(AdminAgentUseCase.AgentId);
///         var edge  = store.FindEdge(user!.Id, agent!.Id, "has.agent");
///         return new SeededAdmin(user, agent, edge);
///     }
/// }
///
/// public interface IFakeGraphReadStoreSync
/// {
///     Node? FindNode(Guid id);
///     Edge? FindEdge(Guid sourceId, Guid targetId, string label);
///     void SeedAdmin(Guid userId, Guid agentId, string edgeLabel);
/// }
///
/// public sealed record SeededAdmin(Node? User, Node? Agent, Edge? Edge);
/// </example>

public abstract class WhenTestingForWithResult<TSut, TResult> : WhenTestingFor<TSut>
{
    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    protected TResult Result { get; private set; } = default!;

    /// <summary>
    /// Executes the operation and returns a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define the specific operation
    /// that produces a result. The implementation should ensure that the returned value is valid and consistent with
    /// the expected behavior of the operation.</remarks>
    /// <returns>The result of the operation, of type <typeparamref name="TResult"/>.</returns>
    protected abstract TResult WhenWithResult();

    /// <inheritdoc/>
    protected sealed override void When() => Result = WhenWithResult();
}