﻿namespace WebVella.Tefter.Web.Store.SpaceState;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceDataViewAction(SetSpaceViewDataAction action, IDispatcher dispatcher)
	{
		Console.WriteLine("HandleSetSpaceDataViewAction");
		dispatcher.Dispatch(new SpaceViewDataChangedAction());
		return Task.CompletedTask;
	}

}