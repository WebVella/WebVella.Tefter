namespace WebVella.Tefter.Messaging;

public class TfDataProviderUpdatedEvent : TfGlobalEvent
{
	public TfDataProvider Payload { get; set; } = null!;

	public TfDataProviderUpdatedEvent() { }

	public TfDataProviderUpdatedEvent(TfDataProvider payload)
	{
		Payload = payload;
	}
}