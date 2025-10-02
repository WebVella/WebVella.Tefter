namespace WebVella.Tefter.Messaging;

public class TfRepositoryFileUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfRepositoryFile Payload { get; set; } = null!;

	public TfRepositoryFileUpdatedEvent() { }

	public TfRepositoryFileUpdatedEvent(TfRepositoryFile payload)
	{
		Payload = payload;
	}
}