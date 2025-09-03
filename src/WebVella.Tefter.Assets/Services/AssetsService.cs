using Microsoft.AspNetCore.Components.Authorization;

namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
    List<TfDataIdentity> GetAllDataIdentities();
    List<TfSharedColumn> GetAllSharedColumns();
    Task<TfUser?> GetCurrentUser(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider);
    TfDataProvider GetDataProvider(Guid providerId);
    List<TfDataProviderIdentity> GetDataProviderIdentities(Guid providerId);
}

internal partial class AssetsService : IAssetsService
{
    public readonly ITfDatabaseService _dbService;
    public readonly ITfService _tfService;

    public AssetsService(
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

	public TfDataProvider GetDataProvider(Guid providerId)
		=> _tfService.GetDataProvider(providerId);
	public List<TfDataProviderIdentity> GetDataProviderIdentities(Guid providerId)
		=> _tfService.GetDataProviderIdentities(providerId);
}
