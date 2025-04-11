using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultPreviewComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultPreviewScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("c940dd21-6151-4c4c-ae5b-e6d21de8b80c");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Text File Template Result Preview";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultPreviewScreenRegionContext RegionContext { get; init; }

	private TextFileTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	private List<ValidationError> _previewValidationErrors = new();


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
				if (result is not TextFileTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type TextFileTemplatePreviewResult");
				}
				_preview = (TextFileTemplatePreviewResult)result;
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