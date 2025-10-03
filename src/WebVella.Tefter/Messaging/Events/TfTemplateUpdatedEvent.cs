namespace WebVella.Tefter.Messaging;

public class TfTemplateUpdatedEvent : TfGlobalEvent
{
	public TfTemplate Payload { get; set; } = null!;

	public TfTemplateUpdatedEvent() { }

	public TfTemplateUpdatedEvent(TfTemplate payload)
	{
		Payload = payload;
	}
}