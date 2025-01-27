namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultComponent : TfBaseComponent, 
	ITfDynamicComponent<TfTemplateProcessorResultComponentContext>,
	ITfComponentScope<TextFileTemplateProcessor>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	public Guid Id { get; init; } = new Guid("e74d6c12-7d1d-4723-be9b-2e1bd1d982e1");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Text File Template Result";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; init; }

	private TextFileTemplateResult _result = null;
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
				if (result is TextFileTemplateResult)
				{
					_result = (TextFileTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}

}