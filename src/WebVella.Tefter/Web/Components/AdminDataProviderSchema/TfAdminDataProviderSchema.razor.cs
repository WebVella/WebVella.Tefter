
namespace WebVella.Tefter.Web.Components.AdminDataProviderSchema;
public partial class TfAdminDataProviderSchema : TfBaseComponent
{
	[Inject] protected IState<DataProviderDetailsState> DataProviderDetailsState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Dispatcher.Dispatch(new EmptyDataProviderDetailsAction());
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
	private void On_GetDataProviderDetailsActionResult(DataProviderDetailsChangedAction action)
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
			ActionSubscriber.SubscribeToAction<DataProviderDetailsChangedAction>(this, On_GetDataProviderDetailsActionResult);
			Dispatcher.Dispatch(new GetDataProviderDetailsAction(urlData.DataProviderId.Value));
		}
	}


}