namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Action<SampleGlobalEvent> SampleGlobalEvent;

	private void OnEventReceived(ITfEvent obj)
	{
		if (obj is SampleGlobalEvent)
			SampleGlobalEvent?.Invoke((SampleGlobalEvent)obj);

	}
}
