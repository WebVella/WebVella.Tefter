namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultPreviewComponent : TfBaseComponent, 
	ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>,
	ITfComponentScope<TextFileTemplateProcessor>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }

	public Guid Id { get; init; } = new Guid("c940dd21-6151-4c4c-ae5b-e6d21de8b80c");
	public int PositionRank { get; init; } = 0;
	public string Name { get; init; } = "Text File Template Result Preview";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; init; }

	private TextFileTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	private List<ValidationError> _previewValidationErrors = new();


	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null) throw new Exception("Context is not defined");
		Context.ValidatePreviewResult = _validatePreviewResult;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Context.Template is not null && Context.SpaceData is not null)
			{
				ITfTemplatePreviewResult result = TemplateService.GenerateTemplatePreviewResult(
					templateId: Context.Template.Id,
					spaceDataId: Context.SpaceData.Id,
					tfRecordIds: Context.SelectedRowIds
				);
				if (result is not TextFileTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type TextFileTemplatePreviewResult");
				}
				_preview = (TextFileTemplatePreviewResult)result;
				await Context.PreviewResultChanged.InvokeAsync(_preview);
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