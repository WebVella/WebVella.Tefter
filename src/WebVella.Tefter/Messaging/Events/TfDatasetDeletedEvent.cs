namespace WebVella.Tefter.Messaging;

public class TfDatasetDeletedEvent : TfGlobalEvent
{
	public TfDataset Payload { get; set; } = null!;

	public TfDatasetDeletedEvent() { }

	public TfDatasetDeletedEvent(TfDataset payload)
	{
		Payload = payload;
	}
}