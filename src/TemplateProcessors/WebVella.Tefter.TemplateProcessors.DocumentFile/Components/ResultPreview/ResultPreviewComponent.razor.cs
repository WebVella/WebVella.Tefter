using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class ResultPreviewComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultPreviewComponentContext>
{
	public Guid Id { get; init; } = new Guid("e6923a63-885f-4201-bab5-701867f7b952");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Document Template Result Preview";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; init; }

	private DocumentFileTemplatePreviewResult _preview = null;
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
				ITfTemplatePreviewResult result = TfService.GenerateTemplatePreviewResult(
					templateId: Context.Template.Id,
					spaceDataId: Context.SpaceData.Id,
					tfRecordIds: Context.SelectedRowIds
				);
				if (result is not DocumentFileTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type DocumentFileTemplatePreviewResult");
				}
				_preview = (DocumentFileTemplatePreviewResult)result;
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