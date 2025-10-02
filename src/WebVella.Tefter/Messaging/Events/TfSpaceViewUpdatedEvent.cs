namespace WebVella.Tefter.Messaging;

public class TfSpaceViewUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpaceView Payload { get; set; } = null!;

	public TfSpaceViewUpdatedEvent() { }

	public TfSpaceViewUpdatedEvent(TfSpaceView payload)
	{
		Payload = payload;
	}
}