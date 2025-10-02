namespace WebVella.Tefter.Messaging;

public class TfTemplateCreatedEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public TfTemplate Payload { get; set; } = null!;

	public TfTemplateCreatedEvent() { }

	public TfTemplateCreatedEvent(TfTemplate payload)
	{
		Payload = payload;
	}
}