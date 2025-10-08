using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfState GetAppState(NavigationManager navigator, TfUser currentUser, string? urlOverride = null);
}

public partial class TfService : ITfService
{
	public TfState GetAppState(NavigationManager navigator, TfUser currentUser, string? urlOverride = null)
		=> navigator.GenerateAppState(this,_metaService, LOC, currentUser, urlOverride);
}