
namespace WebVella.Tefter.Web.Components.AdminDataProviderDetails;
public partial class TfAdminDataProviderDetails : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Dispatcher.Dispatch(new EmptyDataProviderAdminAction());
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_getProvider();
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_getProvider();
	}

	private void _getProvider()
	{
		var urlData = Navigator.GetUrlData();
		if (urlData.DataProviderId is not null)
		{
			var result = UC.GetProvider(urlData.DataProviderId.Value);
			ProcessServiceResponse(result);
			if(result.IsSuccess && result.Value is not null)
				Dispatcher.Dispatch(new SetDataProviderAdminAction(result.Value));
		}
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = ComponentDisplayMode.ReadOnly;
		dict["Value"] = DataProviderDetailsState.Value?.Provider?.SettingsJson;
		return dict;
	}

}