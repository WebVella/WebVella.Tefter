namespace WebVella.Tefter.Messaging;

public class TfTemplateDeletedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfTemplate Payload { get; set; } = null!;

	public TfTemplateDeletedEvent() { }

	public TfTemplateDeletedEvent(TfTemplate payload)
	{
		Payload = payload;
	}
}