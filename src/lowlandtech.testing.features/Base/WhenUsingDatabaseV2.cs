namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Provides a base class for test scenarios involving database contexts with proper lifecycle management.
/// </summary>
/// <remarks>
/// This is an improved version of <see cref="WhenUsingDatabase{TContext}"/> with:
/// - Proper IAsyncLifetime implementation (no blocking in constructor)
/// - CancellationToken support throughout
/// - Automatic database creation and cleanup
/// - Proper async disposal
/// - Consistent naming (Db)
/// </remarks>
/// <typeparam name="TContext">The type of the database context, which must derive from <see cref="DbContext"/>.</typeparam>
public abstract class WhenUsingDatabaseV2<TContext> : IAsyncLifetime
    where TContext : DbContext
{
    /// <summary>
    /// Represents the database context used for data access operations.
    /// </summary>
    protected TContext Db { get; private set; } = null!;

    /// <summary>
    /// Optional cancellation token for test timeout scenarios.
    /// Override this property to provide a custom cancellation token.
    /// </summary>
    protected virtual CancellationToken TestCancellation => CancellationToken.None;

    /// <summary>
    /// Creates a new instance of the context type <typeparamref name="TContext"/>.
    /// Override this method to customize context creation (e.g., with specific options).
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TContext"/>.</returns>
    protected virtual TContext CreateContext() => Activator.CreateInstance<TContext>();

    /// <summary>
    /// Prepares the necessary preconditions or initial state for a test scenario.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task GivenAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Executes the action under test.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task WhenAsync(CancellationToken ct);

    /// <summary>
    /// Provides an opportunity to perform additional cleanup before disposal.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task CleanupAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Controls whether the database should be automatically created.
    /// Default is true. Override to false if using a pre-existing database.
    /// </summary>
    protected virtual bool AutoCreateDatabase => true;

    /// <summary>
    /// Controls whether the database should be automatically deleted on cleanup.
    /// Default is true for test isolation. Override to false to inspect the database after tests.
    /// </summary>
    protected virtual bool AutoDeleteDatabase => true;

    /// <summary>
    /// Initializes the test lifecycle (called by xUnit).
    /// Creates the database context and executes Given ? When.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        var ct = TestCancellation;
        Db = CreateContext();

        if (AutoCreateDatabase)
            await Db.Database.EnsureCreatedAsync(ct).ConfigureAwait(false);

        await GivenAsync(ct).ConfigureAwait(false);
        await WhenAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Cleanup resources (called by xUnit after all tests in this class complete).
    /// Deletes the database and disposes the context.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        if (Db != null)
        {
            await CleanupAsync(TestCancellation).ConfigureAwait(false);

            if (AutoDeleteDatabase)
                await Db.Database.EnsureDeletedAsync().ConfigureAwait(false);

            await Db.DisposeAsync().ConfigureAwait(false);
        }
    }
}
