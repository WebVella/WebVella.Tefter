namespace WebVella.Tefter.UI.Components;
public partial class TucUseTemplateDialog : TfBaseComponent, IDialogContentComponent<TfUseTemplateContext?>
{
	[Parameter] public TfUseTemplateContext? Content { get; set; } = null;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private string? _search = null;
	private bool _loading = true;
	private List<TfTemplate> _templates = new();
	private TfTemplate? _selectedTemplate = null;
	private ITfTemplateProcessorAddon? _processor = null;
	private TfUseTemplateDialogStep _currentStep = TfUseTemplateDialogStep.SelectTemplate;
	private TfTemplateProcessorResultPreviewScreenRegionContext? _resultPreviewComponentContext = null;
	private TfTemplateProcessorResultScreenRegionContext? _resultComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) _error = LOC("Content is null");
		else if (Content.SpaceData is null) _error = LOC("SpaceData is null");
		_initDynamicComponent();
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
		_templates = TfUIService.GetSpaceDataTemplates(Content.SpaceData?.Id ?? Guid.Empty, _search);
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
			if (_resultPreviewComponentContext.ValidatePreviewResult is not null)
			{
				var validationErrors = _resultPreviewComponentContext.ValidatePreviewResult();
				if (validationErrors.Count == 0)
					_currentStep = TfUseTemplateDialogStep.Result;
			}
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

	private void _selectTemplate(TfTemplate template)
	{
		_selectedTemplate = template;
		_initDynamicComponent();

		_resultPreviewComponentContext.Template = _selectedTemplate;
		_resultComponentContext.Template = _selectedTemplate;
		_next();
	}

	private ITfTemplateProcessorAddon _getTemplateProcessorInstance(Type type)
	{
		if (type is not null && type.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
		{
			return (ITfTemplateProcessorAddon)Activator.CreateInstance(type);
		}
		return null;
	}
	private string _getEmbeddedStyles()
	{
		var sb = new StringBuilder();
		sb.AppendLine("<style>");
		sb.AppendLine("html:root {");
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
	private void _customSettingsChanged(string value)
	{
		_resultPreviewComponentContext.CustomSettingsJson = value;
		_resultComponentContext.CustomSettingsJson = value;
	}
	private void _previewResultChanged(ITfTemplatePreviewResult value)
	{
		_resultComponentContext.Preview = value;
	}
	private ITfTemplateProcessorAddon _getProcessor()
	{
		if (_selectedTemplate is null) return null;

		if (_selectedTemplate.ContentProcessorType is not null
			&& _selectedTemplate.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
		{
			return (ITfTemplateProcessorAddon)Activator.CreateInstance(_selectedTemplate.ContentProcessorType);

		}
		return null;

	}

	private void _initDynamicComponent()
	{
		_processor = _getProcessor();
		_resultPreviewComponentContext = new TfTemplateProcessorResultPreviewScreenRegionContext
		{
			Template = null,
			SelectedRowIds = Content.SelectedRowIds,
			SpaceData = Content.SpaceData,
			User = Content.User,
			CustomSettingsJson = null,
			CustomSettingsJsonChanged = EventCallback.Factory.Create<string>(this, _customSettingsChanged),
			PreviewResultChanged = EventCallback.Factory.Create<ITfTemplatePreviewResult>(this, _previewResultChanged),
			ValidatePreviewResult = null,

		};
		_resultComponentContext = new TfTemplateProcessorResultScreenRegionContext
		{
			Template = null,
			SelectedRowIds = Content.SelectedRowIds,
			SpaceData = Content.SpaceData,
			User = Content.User,
			CustomSettingsJson = null,
			Preview = null
		};
		_dynamicComponentScope = _processor is not null
			? new TfScreenRegionScope(_processor.GetType(), null)
			: null;
	}


}

public enum TfUseTemplateDialogStep
{
	SelectTemplate = 0,
	ResultPreview = 1,
	Result = 2,
}
