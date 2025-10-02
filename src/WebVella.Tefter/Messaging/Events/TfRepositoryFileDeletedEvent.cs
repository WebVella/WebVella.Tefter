namespace WebVella.Tefter.Messaging;

public class TfRepositoryFileDeletedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfRepositoryFile Payload { get; set; } = null!;

	public TfRepositoryFileDeletedEvent() { }

	public TfRepositoryFileDeletedEvent(TfRepositoryFile payload)
	{
		Payload = payload;
	}
}