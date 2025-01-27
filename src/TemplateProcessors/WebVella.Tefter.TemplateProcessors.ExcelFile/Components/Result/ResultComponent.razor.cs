using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class ResultComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorResultComponentContext>,
	ITfComponentScope<ExcelFileTemplateProcessor>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	public Guid Id { get; init; } = new Guid("8eed6b14-101b-4fb9-863c-6e520b0196d8");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Excel Template Result";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; init; }

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