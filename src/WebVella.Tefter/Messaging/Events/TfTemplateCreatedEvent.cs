namespace WebVella.Tefter.Messaging;

public class TfTemplateCreatedEvent : TfGlobalEvent
{
	public TfTemplate Payload { get; set; } = null!;

	public TfTemplateCreatedEvent() { }

	public TfTemplateCreatedEvent(TfTemplate payload)
	{
		Payload = payload;
	}
}