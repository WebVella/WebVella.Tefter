using Microsoft.AspNetCore.Components.Authorization;

namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
    Queue<Asset> SharedColumnAssetsProcessQueue { get; }
    List<TfDataIdentity> GetAllDataIdentities();
    List<TfSharedColumn> GetAllSharedColumns();
    Task<TfUser?> GetCurrentUser(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider);
}

internal partial class AssetsService : IAssetsService
{
    private readonly Queue<Asset> _sharedColumnAssetsProcessQueue;
    public readonly ITfDatabaseService _dbService;
    public readonly ITfService _tfService;
    public Queue<Asset> SharedColumnAssetsProcessQueue => _sharedColumnAssetsProcessQueue;

    public AssetsService(
        ITfDatabaseService dbService,
        ITfService tfService)
    {
        _dbService = dbService;
        _tfService = tfService;
        _sharedColumnAssetsProcessQueue = new Queue<Asset>();

    }

    public List<TfDataIdentity> GetAllDataIdentities() => _tfService.GetDataIdentities();

    public List<TfSharedColumn> GetAllSharedColumns() => _tfService.GetSharedColumns();

    public async Task<TfUser?> GetCurrentUser(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
        => await _tfService.GetUserFromCookieAsync(jsRuntime, authStateProvider);

}
