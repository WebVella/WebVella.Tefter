namespace WebVella.Tefter.Messaging;

public class TfTemplateDeletedEvent : TfGlobalEvent
{
	public TfTemplate Payload { get; set; } = null!;

	public TfTemplateDeletedEvent() { }

	public TfTemplateDeletedEvent(TfTemplate payload)
	{
		Payload = payload;
	}
}