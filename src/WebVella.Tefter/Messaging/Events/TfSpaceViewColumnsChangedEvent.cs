namespace WebVella.Tefter.Messaging;

public class TfSpaceViewColumnsChangedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public List<TfSpaceViewColumn> Payload { get; set; } = null!;

	public TfSpaceViewColumnsChangedEvent() { }

	public TfSpaceViewColumnsChangedEvent(List<TfSpaceViewColumn> payload)
	{
		Payload = payload;
	}
}