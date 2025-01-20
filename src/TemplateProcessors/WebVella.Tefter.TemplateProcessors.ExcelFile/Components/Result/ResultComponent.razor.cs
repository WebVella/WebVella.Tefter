using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class ResultComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; set; }

	private ExcelFileTemplateResult _result = null;
	private bool _isLoading = true;
	private bool _showDetails = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null) throw new Exception("Context is not defined");
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (Context.Template is not null && Context.SpaceData is not null)
			{
				ITfTemplateResult result = TemplateService.ProcessTemplate(
					templateId: Context.Template.Id,
					spaceDataId: Context.SpaceData.Id,
					tfRecordIds: Context.SelectedRowIds,
					preview: Context.Preview
				); ;
				if (result is ExcelFileTemplateResult)
				{
					_result = (ExcelFileTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}


}