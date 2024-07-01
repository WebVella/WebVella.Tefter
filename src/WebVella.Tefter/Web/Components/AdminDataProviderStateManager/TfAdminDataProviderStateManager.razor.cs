
namespace WebVella.Tefter.Web.Components.AdminDataProviderStateManager;
public partial class TfAdminDataProviderStateManager : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			Dispatcher.Dispatch(new EmptyDataProviderAdminAction());
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		UC.OnInitialized(initMenu:false);
		_initState(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void _initState(string url)
	{
		var urlData = Navigator.GetUrlData(url);
		if (urlData.DataProviderId is not null)
		{
			var serviceResult = UC.GetProvider(urlData.DataProviderId.Value);
			ProcessServiceResponse(serviceResult);

			if (serviceResult.IsSuccess)
			{
				if (serviceResult.Value is not null)
				{
					Dispatcher.Dispatch(new SetDataProviderAdminAction(
						isBusy: false,
						provider: serviceResult.Value));
					return;
				}
				else
				{
					Navigator.NotFound();
				}
			}
		}
		else
		{
			Dispatcher.Dispatch(new SetDataProviderAdminAction(
						isBusy: false,
						provider: null));
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_initState(e.Location);
	}
}