namespace LowlandTech.Testing.Features.Base;

/// <summary>
/// Provides a base class for testing asynchronous operations with a specified state.
/// </summary>
/// <remarks>This class initializes the state by invoking the <see cref="For"/> method and executes the
/// asynchronous operation defined in <see cref="WhenAsync"/> during construction. Derived classes should implement
/// these methods to define the state and the asynchronous operation being tested.</remarks>
/// <typeparam name="TState">The type representing the state used in the test.</typeparam>
public abstract class WhenTestingForAsync<TState>
{
    /// <summary>
    /// Gets the current state of the system under test (SUT).
    /// </summary>
    protected TState Sut { get; set; }

    /// <summary>
    /// Creates and returns an instance of the state object associated with the current context.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="TState"/> representing the state for the current context.</returns>
    protected abstract TState For();

    /// <summary>
    /// Provides an opportunity to set up preconditions or initial state asynchronously  before the main operation is
    /// executed.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to perform any  necessary setup
    /// logic. By default, it performs no action and completes immediately.</remarks>
    /// <returns></returns>
    protected virtual Task GivenAsync() => Task.CompletedTask;

    /// <summary>
    /// Executes an asynchronous operation defined by the derived class.
    /// </summary>
    /// <remarks>This method must be implemented by a derived class to define the specific behavior of the
    /// asynchronous operation. It is intended to be called within the context of an asynchronous workflow.</remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual Task WhenAsync() => Task.CompletedTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhenTestingForAsync"/> class and performs asynchronous setup
    /// operations.
    /// </summary>
    /// <remarks>This constructor synchronously waits for the completion of asynchronous setup tasks.  Use
    /// caution when calling this constructor, as blocking on asynchronous operations can lead to potential deadlocks 
    /// in certain synchronization contexts, such as UI threads.</remarks>
    protected WhenTestingForAsync()
    {
        Sut = For();
        GivenAsync().GetAwaiter().GetResult();
        WhenAsync().GetAwaiter().GetResult();
    }
}