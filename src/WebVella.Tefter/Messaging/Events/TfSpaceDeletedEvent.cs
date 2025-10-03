namespace WebVella.Tefter.Messaging;

public class TfSpaceDeletedEvent : TfGlobalEvent
{
	public TfSpace Payload { get; set; } = null!;

	public TfSpaceDeletedEvent() { }

	public TfSpaceDeletedEvent(TfSpace payload)
	{
		Payload = payload;
	}
}