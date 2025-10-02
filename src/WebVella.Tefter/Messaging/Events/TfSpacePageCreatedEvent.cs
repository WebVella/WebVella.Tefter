namespace WebVella.Tefter.Messaging;

public class TfSpacePageCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpacePage Payload { get; set; } = null!;

	public TfSpacePageCreatedEvent() { }

	public TfSpacePageCreatedEvent(TfSpacePage payload)
	{
		Payload = payload;
	}
}