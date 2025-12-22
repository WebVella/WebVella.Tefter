namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfSharedColumnEventPayload(TfSharedColumn SharedColumn) : ITfEventPayload;
public record TfSharedColumnCreatedEventPayload(TfSharedColumn SharedColumn) : TfSharedColumnEventPayload(SharedColumn);
public record TfSharedColumnUpdatedEventPayload(TfSharedColumn SharedColumn) : TfSharedColumnEventPayload(SharedColumn);
public record TfSharedColumnDeletedEventPayload(TfSharedColumn SharedColumn) : TfSharedColumnEventPayload(SharedColumn);

