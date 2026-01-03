namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsContent : TfBaseComponent, IAsyncDisposable
{
	private TfTemplate? _template = null;
	private List<TfDatasetAsOption> _spaceDataSelection = new();
	private TfTemplateProcessorDisplaySettingsScreenRegion? _dynamicComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;
	private ITfTemplateProcessorAddon? _processor = null;
	private TfNavigationState _navState = null!;
	private IAsyncDisposable _templateUpdatedEventSubscriber = null!;	
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _templateUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_templateUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfTemplateUpdatedEventPayload>(
			handler:On_TemplateUpdatedEventAsync);		
	}

	private async Task On_TemplateUpdatedEventAsync(string? key, TfTemplateUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		await _init(navState: TfAuthLayout.GetState().NavigationState, template: payload.Template);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState, TfTemplate? template = null)
	{
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
					_template = TfService.GetTemplate(_navState.TemplateId.Value);

			}
			if (_template is null) return;
			_spaceDataSelection = TfService.GetSpaceDataOptionsForTemplate().Where(x => _template.SpaceDataList.Contains(x.Id)).ToList();
			if (_template.ContentProcessorType is not null && _template.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
			{
				_processor = (ITfTemplateProcessorAddon?)Activator.CreateInstance(_template.ContentProcessorType);

			}

			_dynamicComponentContext = new TfTemplateProcessorDisplaySettingsScreenRegion
			{
				Template = _template
			};
			if (_processor is not null)
				_dynamicComponentScope = new TfScreenRegionScope(_processor.GetType(), null);
		}
		finally
		{
			UriInitialized = _navState?.Uri ?? String.Empty;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editTemplate()
	{
		if (_template is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucTemplateManageDialog>(
		_template,
		new ()
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
			TfService.DeleteTemplate(_template.Id);
			var templates = TfService.GetTemplates(type: _template.ResultType);
			ToastService.ShowSuccess(LOC("Template removed"));
			if (templates.Count > 0)
			{
				Navigator.NavigateTo(string.Format(TfConstants.AdminTemplateDetailsPageUrl, templates[0].Id));
			}
			else
			{
				Navigator.NavigateTo(string.Format(TfConstants.AdminTemplatesPageUrl));
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
		_ = await DialogService.ShowDialogAsync<TucTemplateHelpDialog>(
		_template,
		new ()
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
		new ()
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

			_spaceDataSelection = TfService.GetSpaceDataOptionsForTemplate().Where(x => _template.SpaceDataList.Contains(x.Id)).ToList();
		}
	}
}