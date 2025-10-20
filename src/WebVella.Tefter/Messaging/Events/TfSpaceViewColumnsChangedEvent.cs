namespace WebVella.Tefter.Messaging;

public class TfSpaceViewColumnsChangedEvent : TfGlobalEvent
{
	public Guid SpaceViewId { get; set; } = Guid.Empty;
	public List<TfSpaceViewColumn> Payload { get; set; } = null!;

	public TfSpaceViewColumnsChangedEvent() { }

	public TfSpaceViewColumnsChangedEvent(Guid spaceViewId, List<TfSpaceViewColumn> payload)
	{
		SpaceViewId = spaceViewId;
		Payload = payload;
	}
}