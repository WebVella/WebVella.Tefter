
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
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
	}

	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		StateHasChanged();
	}
	
	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = ComponentDisplayMode.ReadOnly;
		dict["Value"] = DataProviderDetailsState.Value?.Provider?.SettingsJson;
		return dict;
	}

}