namespace WebVella.Tefter.Messaging;

public class TfSharedColumnDeletedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfSharedColumn Payload { get; set; } = null!;

	public TfSharedColumnDeletedEvent() { }

	public TfSharedColumnDeletedEvent(TfSharedColumn payload)
	{
		Payload = payload;
	}
}