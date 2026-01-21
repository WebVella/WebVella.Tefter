namespace WebVella.Tefter.UI.EventsBus;

/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfSpaceViewDataEventPayload(Guid SpaceViewId)
	: ITfEventPayload;

public record TfSpaceViewDataReloadEventPayload(Guid SpaceViewId)
	: TfSpaceViewDataEventPayload(SpaceViewId);

public record TfSpaceViewDataUpdatedEventPayload(Guid SpaceViewId, List<Guid> ChangedRows)
	: TfSpaceViewDataEventPayload(SpaceViewId);