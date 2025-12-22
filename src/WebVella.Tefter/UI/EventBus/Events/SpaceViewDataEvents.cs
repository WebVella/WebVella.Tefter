namespace WebVella.Tefter.UI.EventsBus;

/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfSpaceViewDataEventPayload(Guid SpaceViewId, Dictionary<Guid, Dictionary<string, object>> DataDictionary)
	: ITfEventPayload;

public record TfSpaceViewDataUpdatedEventPayload(Guid SpaceViewId, Dictionary<Guid, Dictionary<string, object>> DataDictionary)
	: TfSpaceViewDataEventPayload(SpaceViewId, DataDictionary);