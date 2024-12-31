using System.Diagnostics;

namespace WebVella.Tefter.Web.Store;

public record SetUserStateAction : TfBaseAction
{
	public bool FromEvent { get; } = false;
	public Guid? OldStateHash { get; } = null;
	public TfUserState State { get; }
	internal SetUserStateAction(
		FluxorComponent component,
		TfUserState state,
		Guid? oldStateHash = null,
		bool fromEvent = false
		)
	{
		StateComponent = component;
		State = state;
		OldStateHash = oldStateHash;
		FromEvent = fromEvent;
	}
}

public static partial class UserStateReducers
{
	[ReducerMethod]
	public static TfUserState SetUserStateReducer(TfUserState state, SetUserStateAction action)
	{
		if (action.State.Hash == action.OldStateHash) return state;
		Debug.WriteLine($"========= SetUserStateReducer SESSION: {action.State.SessionId} HASH: {action.State.Hash} OLDHASH: {action.OldStateHash}");
		return action.State;
	}
}

public partial class UserStateEffects
{
	private readonly TfUserEventProvider _eventProvider;
	public UserStateEffects(TfUserEventProvider eventProvider)
	{
		_eventProvider = eventProvider;
	}
	[EffectMethod]
	public async Task SetUserStateEffect(SetUserStateAction action, IDispatcher dispatcher)
	{
		if(action.FromEvent) return;
		if (action.State.Hash == action.OldStateHash) return;

		Debug.WriteLine($"========= SetUserStateEffect SESSION: {action.State.SessionId} HASH: {action.State.Hash} OLDHASH: {action.OldStateHash} ");
		await _eventProvider.PublishEventAsync(new UserStateChangedEvent
		{
			Id = Guid.NewGuid(),
			ComponentId = action.Component?.ComponentId ?? Guid.Empty,
			State = action.State,
		});
	}
}