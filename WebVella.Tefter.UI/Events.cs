namespace WebVella.Tefter.UI;

public abstract class TfBookmarkEventEx(TfBookmark payload) : ITfEventArgs
{
	public readonly TfBookmark Payload => payload;
}
public class TfBookmarkCreatedEventEx(TfBookmark payload) : TfBookmarkEventEx(payload);
public class TfBookmarkUpdatedEventEx(TfBookmark payload) : TfBookmarkEventEx(payload);
public class TfBookmarkDeletedEventEx(TfBookmark payload) : TfBookmarkEventEx(payload);
