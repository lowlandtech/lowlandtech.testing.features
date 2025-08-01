namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Provides a base class for defining test scenarios with a "Given-When-Then" structure.
/// </summary>
/// <remarks>This class is designed to facilitate the setup and execution of test scenarios by providing a
/// structured approach to initializing the system under test, defining preconditions, and executing the action being
/// tested. Derived classes must implement the <see cref="For"/>, <see cref="Given"/>, and <see cref="When"/> methods to
/// define the specific behavior of the test.</remarks>
/// <example>
/// public class WhenInterpolatingGreetingTemplate : WhenTestingFor<string>
/// {
///    private IDictionary _data = null!;
///    private string? _result;
///
///    protected override string For() => "Welcome, {Title} {LastName}!";
///
///    protected override void Given()
///    {
///        _data = new Dictionary<string, string>
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
public abstract class WhenTestingFor<T>
{
    /// <summary>
    /// Represents the system under test (SUT) for the current test context.
    /// </summary>
    /// <remarks>This field is typically initialized with the instance of the type being tested. It is
    /// intended to be used in test scenarios to verify the behavior of the system under test.</remarks>
    protected T Sut { get; set; }

    /// <summary>
    /// Performs an operation and returns a result of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>This method is abstract and must be implemented by a derived class to define the specific
    /// operation and the result to be returned. The implementation should ensure that the returned value is valid and
    /// meaningful for the intended use case.</remarks>
    /// <returns>A result of type <typeparamref name="T"/> representing the outcome of the operation.</returns>
    protected abstract T For();

    /// <summary>
    /// Defines the preconditions or initial state required for the test scenario.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to set up the specific 
    /// conditions necessary for the test. It is called before the execution of the test logic.</remarks>
    protected virtual void Given(){}

    /// <summary>
    /// Executes the action or behavior that is triggered by a specific condition or event.
    /// </summary>
    /// <remarks>This method is intended to be overridden in a derived class to define the specific behavior 
    /// that occurs when the associated condition or event is met. The implementation of this method  should encapsulate
    /// the logic for handling the triggering scenario.</remarks>
    protected virtual void When(){}

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenTestingFor"/> class.
    /// </summary>
    /// <remarks>The constructor automatically calls the <c>Setup</c> method to initialize the
    /// instance.</remarks>
    protected WhenTestingFor()
    {
        Setup();
    }

    /// <summary>
    /// Sets up the test scenario by initializing the system under test (SUT) and executing the given and when steps.
    /// </summary>
    /// <remarks>This method is intended to be called as part of the test initialization process.  It ensures
    /// that the SUT is prepared and the necessary preconditions and actions are applied.</remarks>
    private void Setup()
    {
        Sut = For();
        Given();
        When();
    }
}