namespace WebVella.Tefter.Messaging;

public class TfBookmarkDeletedEvent : TfGlobalEvent
{
	public TfBookmark Payload { get; set; } = null!;

	public TfBookmarkDeletedEvent() { }

	public TfBookmarkDeletedEvent(TfBookmark payload)
	{
		Payload = payload;
	}
}