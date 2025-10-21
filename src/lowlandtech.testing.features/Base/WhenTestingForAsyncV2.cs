namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Provides a base class for testing asynchronous operations with proper lifecycle management.
/// </summary>
/// <remarks>
/// This is an improved version of <see cref="WhenTestingForAsync{T}"/> with:
/// - Proper IAsyncLifetime implementation (no blocking in constructor)
/// - CancellationToken support
/// - Proper async disposal
/// - Consistent naming (Sut)
/// </remarks>
/// <typeparam name="T">The type of the system under test.</typeparam>
public abstract class WhenTestingForAsyncV2<T> : IAsyncLifetime, IAsyncDisposable
{
    /// <summary>
    /// Gets the system under test (SUT).
    /// </summary>
    protected T Sut { get; private set; } = default!;

    /// <summary>
    /// Optional cancellation token for test timeout scenarios.
    /// Override this property to provide a custom cancellation token.
    /// </summary>
    protected virtual CancellationToken TestCancellation => CancellationToken.None;

    /// <summary>
    /// Creates and returns the system under test.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/> representing the SUT.</returns>
    protected abstract T For();

    /// <summary>
    /// Sets up preconditions or initial state asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task GivenAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Executes the action under test asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task WhenAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Provides an opportunity to perform additional cleanup before disposal.
    /// </summary>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task CleanupAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Initializes the test lifecycle (called by xUnit).
    /// Executes For ? Given ? When in sequence.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        var ct = TestCancellation;
        Sut = For();
        await GivenAsync(ct).ConfigureAwait(false);
        await WhenAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Cleanup resources (called by xUnit after all tests in this class complete).
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        await CleanupAsync(TestCancellation).ConfigureAwait(false);

        if (Sut is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        else if (Sut is IDisposable disposable)
            disposable.Dispose();
    }
}
