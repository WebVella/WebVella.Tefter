namespace WebVella.Tefter.UI.EventsBus;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfDataIdentityEventPayload(TfDataIdentity DataIdentity) : ITfEventPayload;
public record TfDataIdentityCreatedEventPayload(TfDataIdentity DataIdentity) : TfDataIdentityEventPayload(DataIdentity);
public record TfDataIdentityUpdatedEventPayload(TfDataIdentity DataIdentity) : TfDataIdentityEventPayload(DataIdentity);
public record TfDataIdentityDeletedEventPayload(TfDataIdentity DataIdentity) : TfDataIdentityEventPayload(DataIdentity);

