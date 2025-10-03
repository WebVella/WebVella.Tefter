namespace WebVella.Tefter.Messaging;

public class TfRoleDeletedEvent : TfGlobalEvent
{
	public TfRole Payload { get; set; } = null!;

	public TfRoleDeletedEvent() { }

	public TfRoleDeletedEvent(TfRole payload)
	{
		Payload = payload;
	}
}