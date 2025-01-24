using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultPreviewComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }

	public Guid Id { get; set; } = new Guid("00c3172f-e887-4fa4-89c5-dc2e1c647425");
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; set; }

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