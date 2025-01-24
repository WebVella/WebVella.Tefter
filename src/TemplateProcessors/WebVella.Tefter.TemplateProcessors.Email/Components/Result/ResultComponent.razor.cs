namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class ResultComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	public Guid Id { get; set; } = new Guid("623a2def-f5f0-466e-9690-6de4bd7584df");
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; set; }

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