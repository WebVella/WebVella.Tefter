namespace WebVella.Tefter.Messaging;

public class TfSharedColumnCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSharedColumn Payload { get; set; } = null!;

	public TfSharedColumnCreatedEvent() { }

	public TfSharedColumnCreatedEvent(TfSharedColumn payload)
	{
		Payload = payload;
	}
}