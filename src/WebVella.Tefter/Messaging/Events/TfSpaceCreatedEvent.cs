namespace WebVella.Tefter.Messaging;

public class TfSpaceCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpace Payload { get; set; } = null!;

	public TfSpaceCreatedEvent() { }

	public TfSpaceCreatedEvent(TfSpace payload)
	{
		Payload = payload;
	}
}