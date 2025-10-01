namespace WebVella.Tefter.TemplateProcessors.Email.Addons;

public partial class ResultComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorResultScreenRegionContext>
{
	[Inject] public ITfService TfService { get; set; } = null!;
	public const string ID = "b0ca06fa-4e26-49c4-a043-c0ec7960ab02";
	public const string NAME = "Email Template Result";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(EmailTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultScreenRegionContext RegionContext { get; set; }

	private EmailTemplateResult _result = null;
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
					preview: RegionContext.Preview
				); ;
				if (result is EmailTemplateResult)
				{
					_result = (EmailTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}
}