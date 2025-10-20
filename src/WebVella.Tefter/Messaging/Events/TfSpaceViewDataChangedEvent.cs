namespace WebVella.Tefter.Messaging;

public class TfSpaceViewDataChangedEvent : TfGlobalEvent
{
	public Guid SpaceViewId { get; set; } = Guid.Empty;
	public Dictionary<Guid, Dictionary<string, object>> Payload { get; set; } = null!;

	public TfSpaceViewDataChangedEvent() { }

	public TfSpaceViewDataChangedEvent(Guid spaceViewId, Dictionary<Guid, Dictionary<string, object>> payload)
	{
		SpaceViewId = spaceViewId;
		Payload = payload;
	}
}