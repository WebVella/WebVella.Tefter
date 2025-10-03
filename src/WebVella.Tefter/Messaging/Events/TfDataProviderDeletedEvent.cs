namespace WebVella.Tefter.Messaging;

public class TfDataProviderDeletedEvent : TfGlobalEvent
{
	public TfDataProvider Payload { get; set; } = null!;

	public TfDataProviderDeletedEvent() { }

	public TfDataProviderDeletedEvent(TfDataProvider payload)
	{
		Payload = payload;
	}
}