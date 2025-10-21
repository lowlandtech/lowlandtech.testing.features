namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Base class for asynchronous tests with result capture.
/// </summary>
/// <remarks>
/// This version improves upon <see cref="WhenTestingForWithResultAsync{TSut, TResult}"/> with:
/// - Proper IAsyncLifetime (no blocking in constructor)
/// - CancellationToken support
/// - Proper async disposal
/// </remarks>
/// <example>
/// public sealed class WhenFetchingUserAsync : WhenTestingForWithResultAsyncV2&lt;UserService, User&gt;
/// {
///     protected override UserService For() => new UserService(_httpClient);
///
///     protected override async Task GivenAsync(CancellationToken ct)
///     {
///         await _database.SeedUserAsync("test@example.com", ct);
///     }
///
///     protected override Task&lt;User&gt; WhenWithResultAsync(CancellationToken ct)
///         => Sut.GetUserAsync("test@example.com", ct);
///
///     [Fact]
///     [Then("User should be returned")]
///     public void ShouldReturnUser()
///     {
///         Result.Should().NotBeNull();
///         Result.Email.Should().Be("test@example.com");
///     }
/// }
/// </example>
/// <typeparam name="TSut">The type of the system under test.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the When operation.</typeparam>
public abstract class WhenTestingForWithResultAsyncV2<TSut, TResult> : WhenTestingForAsyncV2<TSut>
{
    /// <summary>
    /// Gets the result of the When operation.
    /// </summary>
    protected TResult Result { get; private set; } = default!;

    /// <summary>
    /// Executes the operation and returns a result asynchronously.
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
