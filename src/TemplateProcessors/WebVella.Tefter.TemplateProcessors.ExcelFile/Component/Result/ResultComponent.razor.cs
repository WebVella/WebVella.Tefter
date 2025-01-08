using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class ResultComponent : TfBaseComponent, ITfCustomComponent
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private TucUseTemplateContext _context = null;
	private ExcelFileTemplatePreviewResult _preview = null;
	private ExcelFileTemplateResult _result = null;
	private bool _isLoading = true;
	private bool _showDetails = false;

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (Context is not null && Context is TucUseTemplateContext)
			{
				_context = Context as TucUseTemplateContext;
				if(!String.IsNullOrWhiteSpace(Value)){ 
					try{ 
						_preview = JsonSerializer.Deserialize<ExcelFileTemplatePreviewResult>(Value);
					}catch{ }
				}
				if (_context.TemplateId.HasValue && _context.SpaceData is not null)
				{
					ITfTemplateResult result = TemplateService.ProcessTemplate(
						templateId: _context.TemplateId.Value,
						spaceDataId: _context.SpaceData.Id,
						tfRecordIds: _context.SelectedRowIds,
						preview:_preview
					);;
					if (result is ExcelFileTemplateResult)
					{
						_result = (ExcelFileTemplateResult)result;
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