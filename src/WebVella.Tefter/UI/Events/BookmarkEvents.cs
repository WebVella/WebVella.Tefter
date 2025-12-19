namespace WebVella.Tefter.UI;

public abstract class TfBookmarkEventEx(TfBookmark payload) : ITfEventPayload
{
	public TfBookmark Payload { get; init; } = payload;
}
public class TfBookmarkCreatedEventEx(TfBookmark payload) : TfBookmarkEventEx(payload);
public class TfBookmarkUpdatedEventEx(TfBookmark payload) : TfBookmarkEventEx(payload);
public class TfBookmarkDeletedEventEx(TfBookmark payload) : TfBookmarkEventEx(payload);

