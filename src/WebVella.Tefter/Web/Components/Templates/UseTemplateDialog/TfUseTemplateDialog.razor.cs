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
	private ITfTemplateProcessor _processor = null;
	private TfUseTemplateDialogStep _currentStep = TfUseTemplateDialogStep.SelectTemplate;
	private string _templateCustomSettings = null;

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

	private void _next()
	{
		if (_currentStep == TfUseTemplateDialogStep.SelectTemplate)
		{
			if (_selectedTemplate is null)
			{
				ToastService.ShowWarning(LOC("You need to select a template first"));
			}
			else
			{
				_currentStep = TfUseTemplateDialogStep.ResultPreview;
			}
		}
		else if (_currentStep == TfUseTemplateDialogStep.ResultPreview)
		{
			_currentStep = TfUseTemplateDialogStep.Result;
		}
	}

	private void _back()
	{
		if (_currentStep == TfUseTemplateDialogStep.SelectTemplate)
		{

		}
		else if (_currentStep == TfUseTemplateDialogStep.ResultPreview)
		{
			_currentStep = TfUseTemplateDialogStep.SelectTemplate;
		}
	}

	private void _selectTemplate(TucTemplate template)
	{
		_selectedTemplate = template;
		if (_selectedTemplate is not null)
		{
			_processor = _getTemplateProcessorInstance(_selectedTemplate.ContentProcessorType);
		}
		_next();
	}

	private ITfTemplateProcessor _getTemplateProcessorInstance(Type type)
	{
		if (type is not null && type.GetInterface(nameof(ITfTemplateProcessor)) != null)
		{
			return (ITfTemplateProcessor)Activator.CreateInstance(type);
		}
		return null;
	}
	private string _getEmbeddedStyles()
	{
		var sb = new StringBuilder();
		sb.AppendLine("<style>");
		sb.AppendLine(":root {");
		sb.AppendLine($"--tf-grid-row-selected: var(--neutral-layer-3);");
		sb.AppendLine("}");
		sb.AppendLine("</style>");
		return sb.ToString();
	}

	private void onSearch(string value)
	{
		_search = value;
		_getTemplates();
	}
	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = TfComponentMode.Update;
		dict["Value"] = _templateCustomSettings;
		dict["ValueChanged"] = EventCallback.Factory.Create<string>(this, _settingsChanged);
		dict["Context"] = Content with {TemplateId = _selectedTemplate?.Id};
		return dict;
	}

	private void _settingsChanged(string value)
	{
		_templateCustomSettings = value;
	}
}

public enum TfUseTemplateDialogStep
{
	SelectTemplate = 0,
	ResultPreview = 1,
	Result = 2,
}
