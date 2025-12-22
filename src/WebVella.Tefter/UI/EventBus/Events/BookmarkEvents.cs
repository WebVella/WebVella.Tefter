namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfBookmarkEventPayload(TfBookmark Bookmark) : ITfEventPayload;
public record TfBookmarkCreatedEventPayload(TfBookmark Bookmark) : TfBookmarkEventPayload(Bookmark);
public record TfBookmarkUpdatedEventPayload(TfBookmark Bookmark) : TfBookmarkEventPayload(Bookmark);
public record TfBookmarkDeletedEventPayload(TfBookmark Bookmark) : TfBookmarkEventPayload(Bookmark);

