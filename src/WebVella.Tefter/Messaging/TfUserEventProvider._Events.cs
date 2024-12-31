namespace WebVella.Tefter.Messaging;


public partial class TfUserEventProvider : IAsyncDisposable
{
	public event Action<UserLogoutEvent> UserLogout;
	public event Action<UserStateChangedEvent> UserStateChanged;

	private void OnEventReceived(ITfEvent obj)
	{
		if (obj is UserLogoutEvent)
			UserLogout?.Invoke((UserLogoutEvent)obj);
		if (obj is UserStateChangedEvent)
			UserStateChanged?.Invoke((UserStateChangedEvent)obj);
	}
}
