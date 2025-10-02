namespace WebVella.Tefter.Messaging;

public class TfUserUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfUser Payload { get; set; } = null!;

	public TfUserUpdatedEvent() { }

	public TfUserUpdatedEvent(TfUser payload)
	{
		Payload = payload;
	}
}