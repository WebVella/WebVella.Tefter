namespace WebVella.Tefter.Messaging;

public class TfRepositoryFileDeletedEvent : TfGlobalEvent
{
	public TfRepositoryFile Payload { get; set; } = null!;

	public TfRepositoryFileDeletedEvent() { }

	public TfRepositoryFileDeletedEvent(TfRepositoryFile payload)
	{
		Payload = payload;
	}
}