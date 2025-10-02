namespace WebVella.Tefter.Messaging;

public class TfRoleCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfRole Payload { get; set; } = null!;

	public TfRoleCreatedEvent() { }

	public TfRoleCreatedEvent(TfRole payload)
	{
		Payload = payload;
	}
}