using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfState GetAppState(NavigationManager navigator, TfUser currentUser, string? urlOverride = null, TfState? oldState = null, TfSpace? updatedSpace = null);
}

public partial class TfService : ITfService
{
	public TfState GetAppState(NavigationManager navigator, TfUser currentUser, string? urlOverride = null, TfState? oldState = null, TfSpace? updatedSpace = null)
		=> navigator.GenerateAppState(this,_metaService, LOC, currentUser, urlOverride,oldState, updatedSpace);
}