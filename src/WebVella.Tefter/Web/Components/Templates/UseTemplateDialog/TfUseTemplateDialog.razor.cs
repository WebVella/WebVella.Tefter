namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.UseTemplateDialog.TfUseTemplateDialog", "WebVella.Tefter")]
public partial class TfUseTemplateDialog : TfBaseComponent, IDialogContentComponent<TucUseTemplateContext>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucUseTemplateContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private string _search = null;
	private bool _loading = true;
	private bool _stepLoading = false;
	private List<TucTemplate> _templates = new();
	private TucTemplate _selectedTemplate = null;
	private TfUseTemplateDialogStep _currentStep = TfUseTemplateDialogStep.SelectTemplate;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) _error = LOC("Content is null");
		else if (Content.SpaceData is null) _error = LOC("SpaceData is null");
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_getTemplates();
			_loading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _getTemplates()
	{
		_templates = UC.GetSpaceDataTemplates(Content.SpaceData?.Id ?? Guid.Empty, _search);
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _next(){ 
		if(_currentStep == TfUseTemplateDialogStep.SelectTemplate){ 
			if(_selectedTemplate is null){ 
				ToastService.ShowWarning(LOC("You need to select a template first"));
			}
			else{ 
				_currentStep = TfUseTemplateDialogStep.ResultPreview;
			}
		}
		else if(_currentStep == TfUseTemplateDialogStep.ResultPreview){
		
		}
	}

	private async Task _back(){ 
		
	}

	private async Task _selectTemplate(TucTemplate template){ 
		_selectedTemplate = template;
	}

	private string _getEmbeddedStyles()
	{
		var sb = new StringBuilder();
		sb.AppendLine("<style>");
		sb.AppendLine(":root {");
		sb.AppendLine($"--tf-grid-row-selected: {Content.SpaceGridSelectedColor};");
		sb.AppendLine($"--space-color: {Content.SpaceColorString};");
		sb.AppendLine($"--accent-base-color: {Content.SpaceColorString};");
		sb.AppendLine($"--accent-fill-rest: {Content.SpaceColorString} !important;");
		sb.AppendLine($"--tf-grid-border-color: {Content.SpaceBorderColor};");
		sb.AppendLine("}");
		sb.AppendLine("</style>");
		return sb.ToString();
	}
	private void onSearch(string value)
	{
		_search = value;
		_getTemplates();
	}
}

public enum TfUseTemplateDialogStep{ 
	SelectTemplate = 0,
	ResultPreview = 1,
	Result = 2,
}
