namespace WebVella.Tefter;

//null logger until we get to time to attach logging
public sealed class NullLogger : ILogger
{
	public IDisposable BeginScope<TState>(TState state) => default!;

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
		Func<TState, Exception, string> formatter)
	{
	}
}
