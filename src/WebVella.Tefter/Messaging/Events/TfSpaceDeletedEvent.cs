namespace WebVella.Tefter.Messaging;

public class TfSpaceDeletedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpace Payload { get; set; } = null!;

	public TfSpaceDeletedEvent() { }

	public TfSpaceDeletedEvent(TfSpace payload)
	{
		Payload = payload;
	}
}