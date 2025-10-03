namespace WebVella.Tefter.Messaging;

public class TfSpaceViewColumnsChangedEvent : TfGlobalEvent
{
	public List<TfSpaceViewColumn> Payload { get; set; } = null!;

	public TfSpaceViewColumnsChangedEvent() { }

	public TfSpaceViewColumnsChangedEvent(List<TfSpaceViewColumn> payload)
	{
		Payload = payload;
	}
}