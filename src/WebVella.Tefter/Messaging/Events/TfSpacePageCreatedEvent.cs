namespace WebVella.Tefter.Messaging;

public class TfSpacePageCreatedEvent : TfGlobalEvent
{
	public TfSpacePage Payload { get; set; } = null!;

	public TfSpacePageCreatedEvent() { }

	public TfSpacePageCreatedEvent(TfSpacePage payload)
	{
		Payload = payload;
	}
}