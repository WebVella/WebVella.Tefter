namespace WebVella.Tefter.Models;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfSpaceEventPayload(TfSpace Space) : ITfEventPayload;
public record TfSpaceCreatedEventPayload(TfSpace Space) : TfSpaceEventPayload(Space);
public record TfSpaceUpdatedEventPayload(TfSpace Space) : TfSpaceEventPayload(Space);
public record TfSpaceDeletedEventPayload(TfSpace Space) : TfSpaceEventPayload(Space);

