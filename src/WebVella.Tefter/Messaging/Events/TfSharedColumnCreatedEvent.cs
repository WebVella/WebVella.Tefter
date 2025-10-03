namespace WebVella.Tefter.Messaging;

public class TfSharedColumnCreatedEvent : TfGlobalEvent
{
	public TfSharedColumn Payload { get; set; } = null!;

	public TfSharedColumnCreatedEvent() { }

	public TfSharedColumnCreatedEvent(TfSharedColumn payload)
	{
		Payload = payload;
	}
}