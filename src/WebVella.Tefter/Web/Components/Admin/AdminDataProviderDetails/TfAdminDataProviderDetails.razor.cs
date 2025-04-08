﻿
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderDetails.TfAdminDataProviderDetails", "WebVella.Tefter")]
public partial class TfAdminDataProviderDetails : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private TfDataProviderDisplaySettingsScreenRegion _dynamicComponentContext = null;
	private TfScreenRegionScope _dynamicComponentScope = null;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_initDynamicComponent();
		ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		_initDynamicComponent();
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_initDynamicComponent();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _initDynamicComponent()
	{
		if (TfAppState.Value.AdminDataProvider is not null)
		{
			_dynamicComponentContext = new TfDataProviderDisplaySettingsScreenRegion
			{
				SettingsJson = TfAppState.Value.AdminDataProvider.SettingsJson
			};
			_dynamicComponentScope = new TfScreenRegionScope(TfAppState.Value.AdminDataProvider.ProviderType.Model.GetType(), null);
		}
	}

}