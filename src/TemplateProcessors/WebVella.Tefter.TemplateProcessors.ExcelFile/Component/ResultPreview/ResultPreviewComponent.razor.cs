using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class ResultPreviewComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; set; }

	private TucUseTemplateContext _context = null;
	private ExcelFileTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null) throw new Exception("Context is not defined");
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{

			_context = Context.Data;
			//if (_context.TemplateId.HasValue && _context.SpaceData is not null)
			//{
			//	ITfTemplatePreviewResult result = TemplateService.GenerateTemplatePreviewResult(
			//		templateId: _context.TemplateId.Value,
			//		spaceDataId: _context.SpaceData.Id,
			//		tfRecordIds: _context.SelectedRowIds
			//	);
			//	if (result is ExcelFileTemplatePreviewResult)
			//	{
			//		_preview = (ExcelFileTemplatePreviewResult)result;
			//		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_preview));
			//	}
			//}

			_isLoading = false;
			StateHasChanged();
		}
	}

	public List<ValidationError> Validate()
	{
		return new List<ValidationError>();
	}
}