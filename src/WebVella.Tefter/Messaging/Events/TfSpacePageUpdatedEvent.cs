namespace WebVella.Tefter.Messaging;

public class TfSpacePageUpdatedEvent : TfGlobalEvent
{
	public TfSpacePage Payload { get; set; } = null!;

	public TfSpacePageUpdatedEvent() { }

	public TfSpacePageUpdatedEvent(TfSpacePage payload)
	{
		Payload = payload;
	}
}