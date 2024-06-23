namespace WebVella.Tefter.Messaging;


public partial class UserEventProvider : IAsyncDisposable
{
	public event Action<UserLogoutEvent> UserLogout;

	private void OnEventReceived(ITfEvent obj)
	{
		if (obj is UserLogoutEvent)
			UserLogout?.Invoke((UserLogoutEvent)obj);

	}
}
