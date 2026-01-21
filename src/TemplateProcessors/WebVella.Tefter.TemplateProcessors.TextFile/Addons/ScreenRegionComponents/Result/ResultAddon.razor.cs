namespace WebVella.Tefter.TemplateProcessors.TextFile.Addons;

public partial class ResultAddon : TfBaseComponent, 
	ITfScreenRegionAddon<TfTemplateProcessorResultScreenRegion>
{
	[Inject] public ITfService TfService { get; set; } = null!;
	public const string ID = "e74d6c12-7d1d-4723-be9b-2e1bd1d982e1";
	public const string NAME = "Text File Template Result";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegion RegionContext { get; set; }

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
					tfDatasetIds: RegionContext.RelatedDatasetIds,
					tfSpaceIds: RegionContext.RelatedSpaceIds,				
					sessionId:RegionContext.SessionId,
					userId:RegionContext.User.Id,
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