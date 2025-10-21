using Xunit;
using TestContext = Bunit.TestContext; // Keep for other usages if needed

namespace LowlandTech.Testing.Features.Base;

public abstract class WhenTestingComponentAsync<T> : TestContext, IAsyncLifetime
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WhenTestingComponentAsync{T}"/> class.
    /// Sets up bUnit's JSInterop in Loose mode for component testing.
    /// </summary>
    public WhenTestingComponentAsync()
    {
        try { JSInterop.Mode = JSRuntimeMode.Loose; } catch { }
    }

    /// <summary>System under test (set in ForAsync).</summary>
    protected T Cut { get; private set; } = default!;

    /// <summary>Optional per-test CancellationToken.</summary>
    protected virtual CancellationToken TestCancellation => CancellationToken.None;

    /// <summary>Create/resolve the SUT.</summary>
    protected abstract Task<T> ForAsync(CancellationToken ct);

    /// <summary>Arrange preconditions.</summary>
    protected virtual Task GivenAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>Act: perform the behavior under test.</summary>
    protected virtual Task WhenAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>Runs For/Given/When in order, awaited.</summary>
    public async ValueTask InitializeAsync()
    {
        var ct = TestCancellation;
        Cut = await ForAsync(ct).ConfigureAwait(false);
        await GivenAsync(ct).ConfigureAwait(false);
        await WhenAsync(ct).ConfigureAwait(false);
    }

    /// <summary>Override if you need async cleanup.</summary>
    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
