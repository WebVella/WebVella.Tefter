namespace WebVella.Tefter.Messaging;

public class TfSpacePageDeletedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSpacePage Payload { get; set; } = null!;

	public TfSpacePageDeletedEvent() { }

	public TfSpacePageDeletedEvent(TfSpacePage payload)
	{
		Payload = payload;
	}
}