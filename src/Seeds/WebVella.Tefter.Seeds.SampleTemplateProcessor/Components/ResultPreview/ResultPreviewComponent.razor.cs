namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Components;

public partial class ResultPreviewComponent : TfBaseComponent,
	ITfRegionComponent<TfTemplateProcessorResultPreviewScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("9f9d348d-53eb-4288-a169-69c225847b6b");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample Template Result Preview";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
		new TfScreenRegionScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultPreviewScreenRegionContext RegionContext { get; init; }

	private SampleTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	private List<ValidationError> _previewValidationErrors = new();
	private int _itemPosition = 1;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null) throw new Exception("Context is not defined");
		RegionContext.ValidatePreviewResult = _validatePreviewResult;
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (RegionContext.Template is not null && RegionContext.SpaceData is not null)
			{
				ITfTemplatePreviewResult result = TfService.GenerateTemplatePreviewResult(
					templateId: RegionContext.Template.Id,
					spaceDataId: RegionContext.SpaceData.Id,
					tfRecordIds: RegionContext.SelectedRowIds
				);
				if (result is not SampleTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type SampleTemplatePreviewResult");
				}
				_preview = (SampleTemplatePreviewResult)result;
				await RegionContext.PreviewResultChanged.InvokeAsync(_preview);
			}

			_isLoading = false;
			StateHasChanged();
		}
	}
	private List<ValidationError> _validatePreviewResult()
	{
		_previewValidationErrors = new List<ValidationError>();
		return _previewValidationErrors;
	}

}