namespace WebVella.Tefter.Messaging;

public class TfRepositoryFileCreatedEvent : TfGlobalEvent
{
	public TfRepositoryFile Payload { get; set; } = null!;

	public TfRepositoryFileCreatedEvent() { }

	public TfRepositoryFileCreatedEvent(TfRepositoryFile payload)
	{
		Payload = payload;
	}
}