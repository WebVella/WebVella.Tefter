namespace WebVella.Tefter.Messaging;

public class TfUserCreatedEvent : TfGlobalEvent
{
	public TfUser Payload { get; set; } = null!;

	public TfUserCreatedEvent() { }

	public TfUserCreatedEvent(TfUser payload)
	{
		Payload = payload;
	}
}