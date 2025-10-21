namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Provides a base class for defining test scenarios with a "Given-When-Then" structure.
/// This version implements IDisposable for proper resource cleanup.
/// </summary>
/// <remarks>
/// This is an improved version of <see cref="WhenTestingFor{T}"/> with:
/// - Proper disposal pattern
/// - Consistent naming (Sut)
/// - Better null handling
/// - Virtual cleanup hooks
/// </remarks>
/// <example>
/// public class WhenInterpolatingGreetingTemplate : WhenTestingForV2&lt;string&gt;
/// {
///    private IDictionary _data = null!;
///    private string? _result;
///
///    protected override string For() => "Welcome, {Title} {LastName}!";
///
///    protected override void Given()
///    {
///        _data = new Dictionary&lt;string, string&gt;
///        {
///            { "Title", "Dr." },
///            { "LastName", "Who" }
///        };
///    }
///
///    protected override void When()
///    {
///        _result = Sut.Interpolate(_data);
///    }
///
///    [Fact]
///    public void ItShouldInterpolateTitleAndLastName()
///    {
///        _result.Should().Be("Welcome, Dr. Who!");
///    }
/// }
/// </example>
/// <typeparam name="T">The type of the system under test.</typeparam>
public abstract class WhenTestingForV2<T> : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Represents the system under test (SUT) for the current test context.
    /// </summary>
    protected T Sut { get; set; } = default!;

    /// <summary>
    /// Creates and returns the system under test.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/> representing the SUT.</returns>
    protected abstract T For();

    /// <summary>
    /// Defines the preconditions or initial state required for the test scenario.
    /// </summary>
    /// <remarks>
    /// Override this method in derived classes to set up the specific conditions
    /// necessary for the test. It is called after <see cref="For"/> and before <see cref="When"/>.
    /// </remarks>
    protected virtual void Given() { }

    /// <summary>
    /// Executes the action or behavior under test.
    /// </summary>
    /// <remarks>
    /// Override this method to define the behavior that occurs when the test scenario is executed.
    /// This is called after <see cref="Given"/> during initialization.
    /// </remarks>
    protected virtual void When() { }

    /// <summary>
    /// Provides an opportunity to perform additional cleanup before disposal.
    /// </summary>
    /// <remarks>
    /// Override this method to add custom cleanup logic. This is called before
    /// attempting to dispose the SUT.
    /// </remarks>
    protected virtual void Cleanup() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenTestingForV2{T}"/> class.
    /// </summary>
    /// <remarks>
    /// The constructor automatically calls the Given-When-Then workflow:
    /// 1. For() - Creates the SUT
    /// 2. Given() - Sets up preconditions
    /// 3. When() - Executes the action under test
    /// </remarks>
    protected WhenTestingForV2()
    {
        Sut = For();
        Given();
        When();
    }

    /// <summary>
    /// Releases all resources used by the current instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Cleanup();
            
            if (Sut is IDisposable disposable)
                disposable.Dispose();
        }

        _disposed = true;
    }
}
