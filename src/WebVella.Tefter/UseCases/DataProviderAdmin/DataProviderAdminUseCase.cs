namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly NavigationManager _navigationManager;
	public DataProviderAdminUseCase(
	ITfDataProviderManager dataProviderManager
	, NavigationManager navigationManager)
	{
		_dataProviderManager = dataProviderManager;
		_navigationManager = navigationManager;
	}

	public void OnInitialized(
		bool initMenu
	)
	{
		if (initMenu) InitMenu();
	}

}
