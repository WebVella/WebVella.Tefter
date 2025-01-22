using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class ResultPreviewComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; set; }

	private TextContentTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	private List<ValidationError> _previewValidationErrors = new();
	private int _itemPosition = 1;
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
				if (result is not TextContentTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type TextContentTemplatePreviewResult");
				}
				_preview = (TextContentTemplatePreviewResult)result;
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