namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewHeader.TfSpaceViewHeader", "WebVella.Tefter")]
public partial class TfSpaceViewHeader : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private string _generatePresetPathHtml()
	{
		if (TfRouteState.Value.SpaceViewPresetId is null || TfAppState.Value.SpaceView.Presets.Count == 0) return "";

		var preset = TfAppState.Value.SpaceView.Presets.GetPresetById(TfRouteState.Value.SpaceViewPresetId.Value);
		if (preset is null) return "";

		var result = new StringBuilder();
		var path = new List<TucSpaceViewPreset>();
		ModelHelpers.FillPresetPathById(TfAppState.Value.SpaceView.Presets, TfRouteState.Value.SpaceViewPresetId.Value, path);
		if(path.Count == 0) return "";
		path.Reverse();
		foreach (var item in path)
		{
			result.Append($"<span class=\"page-header-divider\">:</span>");
			result.Append(item.Name);
		}
		return result.ToString();

	}
}