namespace WebVella.Tefter.Messaging;

public class TfRepositoryFileUpdatedEvent : TfGlobalEvent
{
	public TfRepositoryFile Payload { get; set; } = null!;

	public TfRepositoryFileUpdatedEvent() { }

	public TfRepositoryFileUpdatedEvent(TfRepositoryFile payload)
	{
		Payload = payload;
	}
}