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

public partial class TfGlobalEventProvider : IAsyncDisposable, IDisposable
{
	private const string GLOBAL_CHANNEL = "GLOBAL_CHANNEL";
	private readonly ITfEventBus _eventBus;
	private readonly AuthenticationStateProvider _authStateProvider;
	private readonly IServiceProvider _serviceProvider;

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
		var authState = await _authStateProvider.GetAuthenticationStateAsync();
		if (authState is null || authState.User.Identity is null || !authState.User.Identity.IsAuthenticated)
			return;

		var currentUser = ((TfIdentity)authState.User.Identity).User;
		if (currentUser is null) return;
		globalEvent.UserId = currentUser.Id;
		await _eventBus.PublishEventAsync(globalEvent);
	}

	public async ValueTask DisposeAsync()
	{
		await _eventBus.DisposeAsync();
	}

	public void Dispose()
	{
		_eventBus.Dispose();
	}
}