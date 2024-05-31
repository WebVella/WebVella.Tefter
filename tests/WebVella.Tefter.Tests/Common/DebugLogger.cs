namespace WebVella.Tefter.Tests.Common;

public sealed class DebugLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        Debug.WriteLine($"[{eventId.Id,2}: {logLevel,-12}]");
        Debug.Write($"{formatter(state, exception)}");
        Debug.WriteLine("");
    }
}
