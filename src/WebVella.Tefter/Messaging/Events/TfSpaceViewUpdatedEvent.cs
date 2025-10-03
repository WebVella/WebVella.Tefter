namespace WebVella.Tefter.Messaging;

public class TfSpaceViewUpdatedEvent : TfGlobalEvent
{
	public TfSpaceView Payload { get; set; } = null!;

	public TfSpaceViewUpdatedEvent() { }

	public TfSpaceViewUpdatedEvent(TfSpaceView payload)
	{
		Payload = payload;
	}
}