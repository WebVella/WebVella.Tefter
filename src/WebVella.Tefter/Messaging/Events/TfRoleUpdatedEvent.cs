namespace WebVella.Tefter.Messaging;

public class TfRoleUpdatedEvent : TfGlobalEvent
{
	public TfRole Payload { get; set; } = null!;

	public TfRoleUpdatedEvent() { }

	public TfRoleUpdatedEvent(TfRole payload)
	{
		Payload = payload;
	}
}