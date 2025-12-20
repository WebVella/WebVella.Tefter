namespace WebVella.Tefter.Models;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfUserEventPayload(TfUser User) : ITfEventPayload;
public record TfUserCreatedEventPayload(TfUser User) : TfUserEventPayload(User);
public record TfUserUpdatedEventPayload(TfUser User) : TfUserEventPayload(User);
public record TfUserDeletedEventPayload(TfUser User) : TfUserEventPayload(User);

