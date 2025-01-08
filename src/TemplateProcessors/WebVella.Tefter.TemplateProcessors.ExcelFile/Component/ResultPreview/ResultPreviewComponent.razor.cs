using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class ResultPreviewComponent : TfBaseComponent, ITfCustomComponent
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private TucUseTemplateContext _context = null;
	private ExcelFileTemplatePreviewResult _preview = null;
	private bool _isLoading = true;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Context is not null && Context is TucUseTemplateContext)
			{
				_context = Context as TucUseTemplateContext;
				if (_context.TemplateId.HasValue && _context.SpaceData is not null)
				{
					ITfTemplatePreviewResult result = TemplateService.GenerateTemplatePreviewResult(
						templateId: _context.TemplateId.Value,
						spaceDataId: _context.SpaceData.Id,
						tfRecordIds: _context.SelectedRowIds
					);
					if (result is ExcelFileTemplatePreviewResult)
					{
						_preview = (ExcelFileTemplatePreviewResult)result;
						await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_preview));
					}
				}
			}
			_isLoading = false;
			StateHasChanged();
		}
	}

	public List<ValidationError> Validate()
	{
		return new List<ValidationError>();
	}
}