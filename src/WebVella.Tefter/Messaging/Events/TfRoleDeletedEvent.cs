namespace WebVella.Tefter.Messaging;

public class TfRoleDeletedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfRole Payload { get; set; } = null!;

	public TfRoleDeletedEvent() { }

	public TfRoleDeletedEvent(TfRole payload)
	{
		Payload = payload;
	}
}