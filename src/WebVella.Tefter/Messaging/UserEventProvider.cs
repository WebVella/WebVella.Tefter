﻿namespace WebVella.Tefter.Messaging;

public interface IUserEvent : ITfEvent { }

public partial class UserEventProvider : IAsyncDisposable
{
	private readonly User _currentUser;
	private readonly ITfEventBus _eventBus;
	private readonly AuthenticationStateProvider _authStateProvider;

	public UserEventProvider(
		ITfEventBus eventBus,
		AuthenticationStateProvider authStateProvider)
	{
		_eventBus = eventBus;
		_authStateProvider = authStateProvider;
		var authState = authStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
		if (authState.User.Identity != null && authState.User.Identity.IsAuthenticated)
		{
			_currentUser = ((TfIdentity)authState.User.Identity).User;
			_eventBus.JoinChannelsAsync(_currentUser.Id.ToString());
			_eventBus.OnEvent += OnEventReceived;
		}
	}

	public async Task PublishEventAsync(IUserEvent userEvent)
	{
		await _eventBus.PublishEventAsync(userEvent);
	}

	public async ValueTask DisposeAsync()
	{
		await _eventBus.DisposeAsync();
	}
}
