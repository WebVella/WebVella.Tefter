namespace WebVella.Tefter.Messaging;

public class TfRepositoryFileCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfRepositoryFile Payload { get; set; } = null!;

	public TfRepositoryFileCreatedEvent() { }

	public TfRepositoryFileCreatedEvent(TfRepositoryFile payload)
	{
		Payload = payload;
	}
}