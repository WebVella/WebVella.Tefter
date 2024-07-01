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
		bool initForm,
		bool initMenu
	)
	{
		if(initForm) InitForm();
		if (initMenu) InitMenu();
	}

	internal Guid Id { get; private set; } = Guid.NewGuid();

}
