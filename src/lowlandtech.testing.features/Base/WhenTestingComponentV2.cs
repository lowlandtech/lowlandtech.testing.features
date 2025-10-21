using TestContext = Bunit.TestContext;

namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Provides a base class for bUnit component tests with proper lifecycle management.
/// </summary>
/// <remarks>
/// This is an improved version of <see cref="WhenTestingComponent{T}"/> with:
/// - Proper IAsyncLifetime implementation
/// - Standardized naming (Sut instead of Cut)
/// - Proper async disposal
/// - Better null handling
/// </remarks>
/// <typeparam name="T">The type of component being tested.</typeparam>
public abstract class WhenTestingComponentV2<T> : TestContext, IAsyncLifetime
    where T : class
{
    /// <summary>
    /// Represents the system under test (SUT) - the rendered component.
    /// </summary>
    protected T Sut { get; set; } = default!;

    /// <summary>
    /// Optional cancellation token for test timeout scenarios.
    /// </summary>
    protected virtual CancellationToken TestCancellation => CancellationToken.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenTestingComponentV2{T}"/> class.
    /// Sets up bUnit's JSInterop in Loose mode for component testing.
    /// </summary>
    public WhenTestingComponentV2()
    {
        try 
        { 
            JSInterop.Mode = JSRuntimeMode.Loose; 
        } 
        catch 
        { 
            // Ignore JSInterop setup errors in non-Blazor contexts
        }
    }

    /// <summary>
    /// Creates and returns the component under test.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/> representing the component.</returns>
    protected abstract T For();

    /// <summary>
    /// Defines the preconditions or initial state required for the test scenario.
    /// </summary>
    protected virtual void Given() { }

    /// <summary>
    /// Executes the action or behavior under test.
    /// </summary>
    protected virtual void When() { }

    /// <summary>
    /// Provides an opportunity to perform additional cleanup before disposal.
    /// </summary>
    protected virtual Task CleanupAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Initializes the test lifecycle (called by xUnit).
    /// Executes For ? Given ? When in sequence.
    /// </summary>
    public ValueTask InitializeAsync()
    {
        Sut = For();
        Given();
        When();
        return ValueTask.CompletedTask;
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

        // Dispose bUnit TestContext
        base.Dispose();
    }
}
