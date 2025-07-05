namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Provides a base class for test scenarios that involve a database context.
/// </summary>
/// <example>
/// public class WhenSeedingSuperadmin : WhenUsingDatabase<GraphContext>
/// {
///     private Node? _user;
///     private Node? _agent;
///     private Edge? _edge;
/// 
///     protected override async Task GivenAsync()
///     {
///         await Db.Use<AdminAgentUseCase>();
///     }
/// 
///     protected override async Task WhenAsync()
///     {
///         _user = await Db.Nodes.FindAsync(AdminAgentUseCase.UserId);
///         _agent = await Db.Nodes.FindAsync(AdminAgentUseCase.AgentId);
///         _edge = Db.Edges.FirstOrDefault(e =>
///             e.SourceId == _user!.Id &&
///             e.TargetId == _agent!.Id &&
///             e.Label == "has.agent");
///     }
/// 
///     [Fact]
///     public void ItShouldSeedUserWithWalletAndAgent()
///     {
///         _user.ShouldNotBeNull();
///         _agent.ShouldNotBeNull();
///         _user!.Properties["Mnemonic"].ShouldNotBeNullOrWhiteSpace();
///         _user!.Properties["PublicKey"].ShouldNotBeNullOrWhiteSpace();
///         _user!.Properties["Role"].ShouldBe("admin");
///         _agent!.Properties["AgentFor"].ShouldBe(_user.Id.ToString());
///         _edge.ShouldNotBeNull();
///     }
/// }
/// </example>
/// <remarks>This class is designed to facilitate testing by providing a structured approach to setting up and
/// executing database-related test scenarios. It initializes the database context, executes setup logic defined in <see
/// cref="GivenAsync"/>, and performs the test action defined in <see cref="WhenAsync"/>. Derived classes must implement
/// these abstract methods to define specific test behavior.</remarks>
/// <typeparam name="TContext">The type of the database context, which must derive from <see cref="DbContext"/>.</typeparam>
public abstract class WhenUsingDatabase<TContext> where TContext : DbContext
{
    /// <summary>
    /// Represents the database context used for data access operations.
    /// </summary>
    /// <remarks>This field is intended to be used by derived classes to interact with the database. It must
    /// be initialized before use, typically through dependency injection or a constructor.</remarks>
    protected TContext Db = null!;

    /// <summary>
    /// Creates a new instance of the context type <typeparamref name="TContext"/>.
    /// </summary>
    /// <remarks>This method uses <see cref="Activator.CreateInstance{T}"/> to create an instance of the
    /// context. Override this method in a derived class to customize the creation of the context.</remarks>
    /// <returns>A new instance of <typeparamref name="TContext"/>.</returns>
    protected virtual TContext CreateContext() => Activator.CreateInstance<TContext>();

    /// <summary>
    /// Prepares the necessary preconditions or initial state for a test scenario.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to set up any required state or
    /// dependencies before the test execution. It is called as part of the test lifecycle and should ensure that the
    /// system under test is in the desired initial state.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task GivenAsync();

    /// <summary>
    /// Executes an asynchronous operation that must be implemented by derived classes.
    /// </summary>
    /// <remarks>This method is intended to be overridden in a derived class to define the specific
    /// asynchronous behavior. It is called as part of a larger workflow and should not be invoked directly by external
    /// code.</remarks>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task will complete when the operation is
    /// finished.</returns>
    protected abstract Task WhenAsync();

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenUsingDatabase"/> class and performs asynchronous setup
    /// operations.
    /// </summary>
    /// <remarks>This constructor ensures that the necessary setup operations are completed before the
    /// instance is fully initialized. Note that the asynchronous setup is executed synchronously using <see
    /// cref="Task.GetAwaiter"/>.GetResult(),  which may block the calling thread. Consider wrapping this in a task if
    /// using an asynchronous test framework like xUnit.</remarks>
    protected WhenUsingDatabase()
    {
        Setup().GetAwaiter().GetResult(); // You could wrap in Task if using xUnit's async lifecycle
    }

    /// <summary>
    /// Prepares the necessary context and executes the setup steps for a test scenario.
    /// </summary>
    /// <remarks>This method initializes the database context and sequentially invokes the asynchronous  setup
    /// steps defined in <see cref="GivenAsync"/> and <see cref="WhenAsync"/>.  It is intended to be used as part of a
    /// test preparation process.</remarks>
    /// <returns></returns>
    private async Task Setup()
    {
        Db = CreateContext();
        await GivenAsync();
        await WhenAsync();
    }
}
