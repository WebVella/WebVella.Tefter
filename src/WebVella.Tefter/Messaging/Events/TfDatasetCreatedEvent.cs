namespace WebVella.Tefter.Messaging;

public class TfDatasetCreatedEvent : TfGlobalEvent
{
	public TfDataset Payload { get; set; } = null!;

	public TfDatasetCreatedEvent() { }

	public TfDatasetCreatedEvent(TfDataset payload)
	{
		Payload = payload;
	}
}