namespace WebVella.Tefter.Messaging;

public class TfNavigationStateChangedEvent : TfGlobalEvent
{
	public TfNavigationState Payload { get; set; } = null!;

	public TfNavigationStateChangedEvent() { }

	public TfNavigationStateChangedEvent(TfNavigationState payload)
	{
		Payload = payload;
	}
}