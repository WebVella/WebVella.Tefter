namespace WebVella.Tefter.Messaging;

public interface ITfEvent
{
	Guid Id { get; init; }
}

public class TfEvent : ITfEvent
{
	public Guid Id { get; init; }

	public string Message { get; set; }
}
