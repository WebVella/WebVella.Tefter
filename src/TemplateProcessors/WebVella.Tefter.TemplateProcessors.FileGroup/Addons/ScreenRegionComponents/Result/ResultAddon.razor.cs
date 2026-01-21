namespace WebVella.Tefter.TemplateProcessors.FileGroup.Addons;

public partial class ResultAddon : TfBaseComponent, 
	ITfScreenRegionAddon<TfTemplateProcessorResultScreenRegion>
{
	[Inject] public ITfService TfService { get; set; } = null!;
	public const string ID = "F731C246-A0F2-46DD-8DCA-363D31F0E018";
	public const string NAME = "FileGroup Template Result";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(FileGroupTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegion RegionContext { get; set; }

	private FileGroupTemplateResult _result = null;
	private bool _isLoading = true;
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
					userId:RegionContext.User.Id,
					preview: RegionContext.Preview
				); ;
				if (result is FileGroupTemplateResult)
				{
					_result = (FileGroupTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}
}