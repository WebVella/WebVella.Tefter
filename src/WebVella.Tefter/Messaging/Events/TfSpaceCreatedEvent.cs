namespace WebVella.Tefter.Messaging;

public class TfSpaceCreatedEvent : TfGlobalEvent
{
	public TfSpace Payload { get; set; } = null!;

	public TfSpaceCreatedEvent() { }

	public TfSpaceCreatedEvent(TfSpace payload)
	{
		Payload = payload;
	}
}