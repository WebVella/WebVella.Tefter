using Microsoft.AspNetCore.Components.Authorization;

namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
	List<TfDataIdentity> GetAllDataIdentities();
	List<TfSharedColumn> GetAllSharedColumns();
	Task<TfUser?> GetCurrentUser(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider);
	List<TfDataProviderIdentity> GetDataProviderIdentities(Guid providerId);
}

internal partial class TalkService : ITalkService
{
	public readonly ITfDatabaseService _dbService;
	public readonly ITfService _tfService;

	public TalkService(
		ITfDatabaseService dbService,
		ITfService tfService)
	{
		_dbService = dbService;
		_tfService = tfService;
	}

	public List<TfDataIdentity> GetAllDataIdentities() => _tfService.GetDataIdentities();

	public List<TfSharedColumn> GetAllSharedColumns() => _tfService.GetSharedColumns();

	public async Task<TfUser?> GetCurrentUser(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
		=> await _tfService.GetUserFromCookieAsync(jsRuntime, authStateProvider);
	public List<TfDataProviderIdentity> GetDataProviderIdentities(Guid providerId)
		=> _tfService.GetDataProviderIdentities(providerId);
}
