namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewHeader.TfSpaceViewHeader", "WebVella.Tefter")]
public partial class TfSpaceViewHeader : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private string _generatePresetPathHtml()
	{
		if (TfAppState.Value.Route?.SpaceViewPresetId is null || TfAppState.Value.SpaceView.Presets.Count == 0) return "";

		var preset = TfAppState.Value.SpaceView.Presets.GetPresetById(TfAppState.Value.Route.SpaceViewPresetId.Value);
		if (preset is null) return "";

		var result = new StringBuilder();
		var path = new List<TucSpaceViewPreset>();
		ModelHelpers.FillPresetPathById(TfAppState.Value.SpaceView.Presets, TfAppState.Value.Route.SpaceViewPresetId.Value, path);
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