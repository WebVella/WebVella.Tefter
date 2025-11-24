using System.Diagnostics;

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

	public bool IsUserApplicable(TfBaseComponent component)
	{
		var state = component.TfAuthLayout.GetState();
		if (UserId is not null && UserId.Value == state.User.Id)
			return true;
		return false;
	}
		
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
		//Debug.WriteLine($"CREATED:{_eventBus.GetHashCode()}");
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
		//Debug.WriteLine($"RELESED ASYNC:{_eventBus.GetHashCode()}");
		await _eventBus.DisposeAsync();
	}

	public void Dispose()
	{
		//Debug.WriteLine($"RELESED:{_eventBus.GetHashCode()}");
		_eventBus.Dispose();
	}
}