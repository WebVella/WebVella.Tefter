namespace WebVella.Tefter.Messaging;

public class TfRoleCreatedEvent : TfGlobalEvent
{
	public TfRole Payload { get; set; } = null!;

	public TfRoleCreatedEvent() { }

	public TfRoleCreatedEvent(TfRole payload)
	{
		Payload = payload;
	}
}