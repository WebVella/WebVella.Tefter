namespace WebVella.Tefter.Messaging;

public class TfSpaceUpdatedEvent : TfGlobalEvent
{
	public TfSpace Payload { get; set; } = null!;

	public TfSpaceUpdatedEvent() { }

	public TfSpaceUpdatedEvent(TfSpace payload)
	{
		Payload = payload;
	}
}