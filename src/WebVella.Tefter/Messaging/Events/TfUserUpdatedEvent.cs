namespace WebVella.Tefter.Messaging;

public class TfUserUpdatedEvent : TfGlobalEvent
{
	public TfUser Payload { get; set; } = null!;

	public TfUserUpdatedEvent() { }

	public TfUserUpdatedEvent(TfUser payload)
	{
		Payload = payload;
	}
}