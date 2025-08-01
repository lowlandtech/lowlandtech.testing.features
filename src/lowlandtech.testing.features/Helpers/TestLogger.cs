namespace LowlandTech.Testing.Features.Helpers;

/// <summary>
/// Provides a test implementation of the <see cref="ILogger{T}"/> interface that writes log messages to an <see
/// cref="ITestOutputHelper"/> for use in unit tests.
/// </summary>
/// <remarks>This logger is designed for use in testing scenarios where log output needs to be captured and
/// displayed in test results. It writes log messages and optional exception details to the provided <see
/// cref="ITestOutputHelper"/> instance. All log levels are considered enabled by default.</remarks>
/// <typeparam name="T">The type associated with the logger, typically used for categorization.</typeparam>
/// <param name="output"></param>
public class TestLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to associate with the scope.</typeparam>
    /// <param name="state">The state to associate with the scope. This can be used to pass contextual information.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope on disposal. For implementations that do not support scopes, a
    /// no-op disposable is returned.</returns>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        // In a test logger, we don't need to manage scopes, so we return a no-op disposable.
        return NullScope.Instance;
    }

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Log(logLevel, eventId, state, exception, formatter);
    }

    /// <summary>
    /// Determines whether logging is enabled for the specified log level.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns><see langword="true"/> if logging is enabled for the specified <paramref name="logLevel"/>; otherwise, <see
    /// langword="false"/>.</returns>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>
    /// Logs a message with the specified log level, event ID, state, and optional exception.
    /// </summary>
    /// <remarks>The <paramref name="formatter"/> function is invoked to generate the log message, which is
    /// then written to the output. If an <paramref name="exception"/> is provided, its details are also
    /// logged.</remarks>
    /// <typeparam name="TState">The type of the state object to be logged.</typeparam>
    /// <param name="logLevel">The severity level of the log message.</param>
    /// <param name="eventId">The identifier for the event being logged.</param>
    /// <param name="state">The state object that contains the log message or contextual information.</param>
    /// <param name="exception">An optional exception associated with the log message. Can be <see langword="null"/>.</param>
    /// <param name="formatter">A function that formats the <paramref name="state"/> and <paramref name="exception"/> into a log message string.</param>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        output.WriteLine($"[{logLevel}] {message}");
        if (exception != null)
        {
            output.WriteLine(exception.ToString());
        }
    }

    /// <summary>
    /// Represents a no-op implementation of <see cref="IDisposable"/>.
    /// </summary>
    /// <remarks>This class is used as a placeholder for scenarios where a disposable object is required,  but
    /// no actual resource management or disposal is necessary. It provides a singleton instance  to avoid unnecessary
    /// allocations.</remarks>
    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
}
