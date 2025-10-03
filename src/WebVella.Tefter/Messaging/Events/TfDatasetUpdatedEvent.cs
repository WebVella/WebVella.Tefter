namespace WebVella.Tefter.Messaging;

public class TfDatasetUpdatedEvent : TfGlobalEvent
{
	public TfDataset Payload { get; set; } = null!;

	public TfDatasetUpdatedEvent() { }

	public TfDatasetUpdatedEvent(TfDataset payload)
	{
		Payload = payload;
	}
}