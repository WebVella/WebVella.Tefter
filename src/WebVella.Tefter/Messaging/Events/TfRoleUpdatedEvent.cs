namespace WebVella.Tefter.Messaging;

public class TfRoleUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfRole Payload { get; set; } = null!;

	public TfRoleUpdatedEvent() { }

	public TfRoleUpdatedEvent(TfRole payload)
	{
		Payload = payload;
	}
}