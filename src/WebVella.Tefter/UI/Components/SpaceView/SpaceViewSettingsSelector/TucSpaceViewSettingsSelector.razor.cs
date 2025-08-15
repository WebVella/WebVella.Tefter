namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewSettingsSelector.TucSpaceViewSettingsSelector", "WebVella.Tefter")]
public partial class TucSpaceViewSettingsSelector : TfBaseComponent
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	private bool _open = false;

	private void _init()
	{
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}

	private void _manageView()
	{
		var url = NavigatorExt.AddQueryValueToUri(
			url: string.Format(TfConstants.SpaceViewPageUrl, SpaceView.SpaceId, SpaceView.Id),
			paramName: TfConstants.ReturnUrlQueryName,
			Navigator.GetLocalUrl()
			);
		Navigator.NavigateTo(url);
	}
	private void _manageData()
	{
		var url = NavigatorExt.AddQueryValueToUri(
			url: string.Format(TfConstants.SpaceDataPageUrl, SpaceView.SpaceId, SpaceView.SpaceDataId),
			paramName: TfConstants.ReturnUrlQueryName,
			Navigator.GetLocalUrl()
			);
		Navigator.NavigateTo(url);
	}

}