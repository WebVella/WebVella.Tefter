namespace WebVella.Tefter.Messaging;

public class TfSpaceUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpace Payload { get; set; } = null!;

	public TfSpaceUpdatedEvent() { }

	public TfSpaceUpdatedEvent(TfSpace payload)
	{
		Payload = payload;
	}
}