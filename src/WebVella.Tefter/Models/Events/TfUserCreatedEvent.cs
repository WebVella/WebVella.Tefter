namespace WebVella.Tefter.Messaging;

public class TfUserCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfUser Payload { get; set; } = null!;

	public TfUserCreatedEvent() { }

	public TfUserCreatedEvent(TfUser payload)
	{
		Payload = payload;
	}
}