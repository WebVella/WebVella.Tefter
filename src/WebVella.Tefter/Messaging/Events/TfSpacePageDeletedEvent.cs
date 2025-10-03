namespace WebVella.Tefter.Messaging;

public class TfSpacePageDeletedEvent : TfGlobalEvent
{
	public TfSpacePage Payload { get; set; } = null!;

	public TfSpacePageDeletedEvent() { }

	public TfSpacePageDeletedEvent(TfSpacePage payload)
	{
		Payload = payload;
	}
}