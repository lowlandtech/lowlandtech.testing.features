namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Async-only G/W/T harness for bUnit/xUnit. Nothing runs in the ctor.
/// xUnit awaits <see cref="InitializeAsync"/> before any [Fact].
/// </summary>
/// <example>
/// <code>
/// public sealed class WhenInterpolatingGreetingTemplate_Async
///     : WhenTestingComponentAsync&lt;string&gt;
/// {
///     private IDictionary&lt;string, string&gt; _data = null!;
///     private string? _result;
///
///     protected override Task&lt;string&gt; ForAsync(CancellationToken ct)
///         =&gt; Task.FromResult("Welcome, {Title} {LastName}!");
///
///     protected override Task GivenAsync(CancellationToken ct)
///     {
///         _data = new Dictionary&lt;string, string&gt;
///         {
///             ["Title"] = "Dr.",
///             ["LastName"] = "Who"
///         };
///         return Task.CompletedTask;
///     }
///
///     protected override Task WhenAsync(CancellationToken ct)
///     {
///         _result = Cut.Interpolate(_data);
///         return Task.CompletedTask;
///     }
///
///     [Fact]
///     public void ItShouldInterpolateTitleAndLastName()
///         =&gt; _result.Should().Be("Welcome, Dr. Who!");
/// }
/// </code>
/// </example>
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
    public async Task InitializeAsync()
    {
        var ct = TestCancellation;
        Cut = await ForAsync(ct).ConfigureAwait(false);
        await GivenAsync(ct).ConfigureAwait(false);
        await WhenAsync(ct).ConfigureAwait(false);
    }

    /// <summary>Override if you need async cleanup.</summary>
    public virtual Task DisposeAsync() => Task.CompletedTask;
}
