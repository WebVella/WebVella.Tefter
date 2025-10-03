namespace WebVella.Tefter.Messaging;

public class TfDataProviderCreatedEvent : TfGlobalEvent
{
	public TfDataProvider Payload { get; set; } = null!;

	public TfDataProviderCreatedEvent() { }

	public TfDataProviderCreatedEvent(TfDataProvider payload)
	{
		Payload = payload;
	}
}