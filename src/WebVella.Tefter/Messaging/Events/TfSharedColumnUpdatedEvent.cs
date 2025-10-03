namespace WebVella.Tefter.Messaging;

public class TfSharedColumnUpdatedEvent : TfGlobalEvent
{
	public TfSharedColumn Payload { get; set; } = null!;

	public TfSharedColumnUpdatedEvent() { }

	public TfSharedColumnUpdatedEvent(TfSharedColumn payload)
	{
		Payload = payload;
	}
}