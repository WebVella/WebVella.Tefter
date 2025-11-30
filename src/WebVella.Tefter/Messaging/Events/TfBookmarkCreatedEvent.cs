namespace WebVella.Tefter.Messaging;

public class TfBookmarkCreatedEvent : TfGlobalEvent
{
	public TfBookmark Payload { get; set; } = null!;

	public TfBookmarkCreatedEvent() { }

	public TfBookmarkCreatedEvent(TfBookmark payload)
	{
		Payload = payload;
	}
}