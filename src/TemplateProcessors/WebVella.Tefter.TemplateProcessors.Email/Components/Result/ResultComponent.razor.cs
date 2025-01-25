namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class ResultComponent : TfBaseComponent, 
	ITfDynamicComponent<TfTemplateProcessorResultComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }

	public Guid Id { get; init; } = new Guid("b0ca06fa-4e26-49c4-a043-c0ec7960ab02");
	public int PositionRank { get; init; } = 0;
	public string Name { get; init; } = "Email Template Result";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; init; }

	private EmailTemplateResult _result = null;
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
				if (result is EmailTemplateResult)
				{
					_result = (EmailTemplateResult)result;
				}
			}

			_isLoading = false;
			StateHasChanged();
		}
	}
}