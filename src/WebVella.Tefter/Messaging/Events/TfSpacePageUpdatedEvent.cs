namespace WebVella.Tefter.Messaging;

public class TfSpacePageUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpacePage Payload { get; set; } = null!;

	public TfSpacePageUpdatedEvent() { }

	public TfSpacePageUpdatedEvent(TfSpacePage payload)
	{
		Payload = payload;
	}
}