namespace WebVella.Tefter.Models;
/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfSpacePageEventPayload(TfSpacePage SpacePage) : ITfEventPayload;
public record TfSpacePageCreatedEventPayload(TfSpacePage SpacePage) : TfSpacePageEventPayload(SpacePage);
public record TfSpacePageUpdatedEventPayload(TfSpacePage SpacePage) : TfSpacePageEventPayload(SpacePage);
public record TfSpacePageDeletedEventPayload(TfSpacePage SpacePage) : TfSpacePageEventPayload(SpacePage);

