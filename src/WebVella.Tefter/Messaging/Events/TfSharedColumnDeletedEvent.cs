namespace WebVella.Tefter.Messaging;

public class TfSharedColumnDeletedEvent : TfGlobalEvent
{
	public TfSharedColumn Payload { get; set; } = null!;

	public TfSharedColumnDeletedEvent() { }

	public TfSharedColumnDeletedEvent(TfSharedColumn payload)
	{
		Payload = payload;
	}
}