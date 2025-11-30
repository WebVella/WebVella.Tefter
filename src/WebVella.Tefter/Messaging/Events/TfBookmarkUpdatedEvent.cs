namespace WebVella.Tefter.Messaging;

public class TfBookmarkUpdatedEvent : TfGlobalEvent
{
	public TfBookmark Payload { get; set; } = null!;

	public TfBookmarkUpdatedEvent() { }

	public TfBookmarkUpdatedEvent(TfBookmark payload)
	{
		Payload = payload;
	}
}