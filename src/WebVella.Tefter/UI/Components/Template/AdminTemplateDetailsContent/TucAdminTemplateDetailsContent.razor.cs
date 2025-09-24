namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfTemplateUIService TfTemplateUIService { get; set; } = default!;
	[Inject] public ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private TfTemplate? _template = null;
	private List<TfDataSetAsOption> _spaceDataSelection = new();
	public bool _submitting = false;
	private TfTemplateProcessorDisplaySettingsScreenRegionContext? _dynamicComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;
	private ITfTemplateProcessorAddon? _processor = null;
	private List<ITfTemplateProcessorAddon> _allProcessors = new();
	private TfNavigationState _navState = default!;
	public void Dispose()
	{
		TfTemplateUIService.TemplateUpdated -= On_TemplateUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfTemplateUIService.TemplateUpdated += On_TemplateUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_TemplateUpdated(object? caller, TfTemplate args)
	{
		await _init(template: args);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfTemplate? template = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			_template = null;
			_processor = null;
			if (template is not null && template.Id == _template?.Id)
			{
				_template = template;
			}
			else
			{
				if (_navState.TemplateId is not null)
					_template = TfTemplateUIService.GetTemplate(_navState.TemplateId.Value);

			}
			if (_template is null) return;
			_spaceDataSelection = TfTemplateUIService.GetSpaceDataOptionsForTemplate().Where(x => _template.SpaceDataList.Contains(x.Id)).ToList();
			if (_template.ContentProcessorType is not null && _template.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
			{
				_processor = (ITfTemplateProcessorAddon?)Activator.CreateInstance(_template.ContentProcessorType);

			}

			_dynamicComponentContext = new TfTemplateProcessorDisplaySettingsScreenRegionContext
			{
				Template = _template
			};
			if (_processor is not null)
				_dynamicComponentScope = new TfScreenRegionScope(_processor.GetType(), null);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editTemplate()
	{
		if (_template is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucTemplateManageDialog>(
		_template,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _deleteTemplate()
	{
		if (_template is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this template deleted?")))
			return;
		try
		{
			TfTemplateUIService.DeleteTemplate(_template.Id);
			var templates = TfTemplateUIService.GetTemplates(type: _template.ResultType);
			ToastService.ShowSuccess(LOC("Template removed"));
			if (templates.Count > 0)
			{
				Navigator.NavigateTo(string.Format(TfConstants.AdminTemplatesTemplatePageUrl, (int)_template.ResultType, templates[0].Id));
			}
			else
			{
				Navigator.NavigateTo(string.Format(TfConstants.AdminTemplatesTypePageUrl, (int)_template.ResultType));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task onHelpClick()
	{
		if (_template is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucTemplateHelpDialog>(
		_template,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
	}
	private async Task onUpdateSettingsClick()
	{
		if (_template is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucTemplateSettingsDialog>(
		_template,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			_template = (TfTemplate)result.Data;

			_spaceDataSelection = TfTemplateUIService.GetSpaceDataOptionsForTemplate().Where(x => _template.SpaceDataList.Contains(x.Id)).ToList();
		}
	}
}