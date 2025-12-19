namespace WebVella.Tefter.Models;

/// <summary>
/// Base payload for all bookmark-related events.
/// Use base class to subscribe to all derivatives
/// </summary>
public abstract record TfSpaceViewColumnEventPayload(Guid ColumnId, List<TfSpaceViewColumn> SpaceViewColumns)
	: ITfEventPayload;

public record TfSpaceViewColumnUpdatedEventPayload(Guid ColumnId, List<TfSpaceViewColumn> SpaceViewColumns)
	: TfSpaceViewColumnEventPayload(ColumnId, SpaceViewColumns);