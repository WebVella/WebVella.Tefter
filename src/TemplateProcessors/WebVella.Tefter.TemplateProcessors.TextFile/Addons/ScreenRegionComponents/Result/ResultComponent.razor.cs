namespace WebVella.Tefter.TemplateProcessors.TextFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Addons.ScreenRegionComponents.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultScreenRegionContext>
{
	public const string ID = "e74d6c12-7d1d-4723-be9b-2e1bd1d982e1";
	public const string NAME = "Text File Template Result";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegionContext RegionContext { get; init; }

	private TextFileTemplateResult _result = null;
	private bool _isLoading = true;
	private bool _showDetails = false;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null) throw new Exception("Context is not defined");
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (RegionContext.Template is not null && RegionContext.SpaceData is not null)
			{
				ITfTemplateResult result = TfService.ProcessTemplate(
					templateId: RegionContext.Template.Id,
					spaceDataId: RegionContext.SpaceData.Id,
					tfRecordIds: RegionContext.SelectedRowIds,
					preview: RegionContext.Preview
				); ;
				if (result is TextFileTemplateResult)
				{
					_result = (TextFileTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}

}