namespace LowlandTech.Testing.Features.Helpers;

public class TestLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

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

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
}
