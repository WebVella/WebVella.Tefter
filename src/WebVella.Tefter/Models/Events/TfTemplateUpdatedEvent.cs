namespace WebVella.Tefter.Messaging;

public class TfTemplateUpdatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfTemplate Payload { get; set; } = null!;

	public TfTemplateUpdatedEvent() { }

	public TfTemplateUpdatedEvent(TfTemplate payload)
	{
		Payload = payload;
	}
}