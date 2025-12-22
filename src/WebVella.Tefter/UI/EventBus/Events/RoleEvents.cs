namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfRoleEventPayload(TfRole Role) : ITfEventPayload;
public record TfRoleCreatedEventPayload(TfRole Role) : TfRoleEventPayload(Role);
public record TfRoleUpdatedEventPayload(TfRole Role) : TfRoleEventPayload(Role);
public record TfRoleDeletedEventPayload(TfRole Role) : TfRoleEventPayload(Role);

