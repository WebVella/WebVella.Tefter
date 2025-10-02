namespace WebVella.Tefter.Messaging;

public class TfSharedColumnUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSharedColumn Payload { get; set; } = null!;

	public TfSharedColumnUpdatedEvent() { }

	public TfSharedColumnUpdatedEvent(TfSharedColumn payload)
	{
		Payload = payload;
	}
}