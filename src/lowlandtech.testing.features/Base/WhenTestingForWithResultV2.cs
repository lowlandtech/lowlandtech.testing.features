namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// V2: Base class for synchronous tests with result capture.
/// </summary>
/// <remarks>
/// This version improves upon <see cref="WhenTestingForWithResult{TSut, TResult}"/> with:
/// - Proper disposal pattern
/// - Consistent naming (Sut)
/// - Virtual cleanup hooks
/// </remarks>
/// <example>
/// public sealed class WhenCalculatingSum : WhenTestingForWithResultV2&lt;Calculator, int&gt;
/// {
///     private int _a;
///     private int _b;
///
///     protected override Calculator For() => new Calculator();
///
///     protected override void Given()
///     {
///         _a = 5;
///         _b = 3;
///     }
///
///     protected override int WhenWithResult() => Sut.Add(_a, _b);
///
///     [Fact]
///     [Then("Result should be 8")]
///     public void ShouldReturnCorrectSum()
///     {
///         Result.Should().Be(8);
///     }
/// }
/// </example>
/// <typeparam name="TSut">The type of the system under test.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the When operation.</typeparam>
public abstract class WhenTestingForWithResultV2<TSut, TResult> : WhenTestingForV2<TSut>
{
    /// <summary>
    /// Gets the result of the When operation.
    /// </summary>
    protected TResult Result { get; private set; } = default!;

    /// <summary>
    /// Executes the operation and returns a result.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    protected abstract TResult WhenWithResult();

    /// <inheritdoc/>
    protected sealed override void When()
    {
        Result = WhenWithResult();
    }
}
