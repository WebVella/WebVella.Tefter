using WebVella.Tefter.Authentication;

namespace WebVella.Tefter.Messaging;

public interface IGlobalEvent : ITfEvent
{
	Guid? UserId { get; set; }
}

public class TfGlobalEvent : IGlobalEvent
{
	public Guid Id { get; init; }
	public Guid? UserId { get; set; } = null;

	public bool IsUserApplicable(TfUser? user)
		=> UserId is not null && user is not null && UserId.Value == user.Id;
}

public partial class TfGlobalEventProvider : IAsyncDisposable
{
	private const string GLOBAL_CHANNEL = "GLOBAL_CHANNEL";
	private readonly ITfEventBus _eventBus;
	private readonly AuthenticationStateProvider _authStateProvider;

	public TfGlobalEventProvider(
		ITfEventBus eventBus,
		AuthenticationStateProvider authStateProvider)
	{
		_authStateProvider = authStateProvider;
		_eventBus = eventBus;
		_eventBus.JoinChannelsAsync(GLOBAL_CHANNEL);
		_eventBus.OnEvent += OnEventReceived;
	}

	public async Task PublishEventAsync(IGlobalEvent globalEvent)
	{
		var authState = _authStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
		if (authState is not null && authState.User.Identity != null && authState.User.Identity.IsAuthenticated)
		{
			var _currentUser = ((TfIdentity)authState.User.Identity).User;
			if (_currentUser is not null)
				globalEvent.UserId = _currentUser.Id;
		}

		await _eventBus.PublishEventAsync(globalEvent);
	}

	public async ValueTask DisposeAsync()
	{
		await _eventBus.DisposeAsync();
	}
}