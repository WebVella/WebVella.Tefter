namespace WebVella.Tefter.UI.Components;
public partial class TucTemplateSettingsDialog : TfBaseComponent, IDialogContentComponent<TfTemplate?>
{
	[Parameter] public TfTemplate? Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private string? _form = null;

	private ITfTemplateProcessorAddon? _processor = null;
	private TfTemplateProcessorManageSettingsScreenRegionContext? _dynamicComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Update settings");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save")!.WithColor(Color.Neutral);
		_form = Content.SettingsJson;
		_initDynamicComponent();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			////Check form
			List<ValidationError> settingsErrors = new();
			if (_dynamicComponentContext.Validate is not null)
			{
				settingsErrors = _dynamicComponentContext.Validate();
			}

			if (settingsErrors.Count > 0) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			var template = TfUIService.UpdateTemplateSettings(Content.Id, _form);
			ToastService.ShowSuccess(LOC("Template settings successfully updated"));
			await Dialog.CloseAsync(template);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private void _settingsChanged(string json)
	{
		_form = json;
	}

	private ITfTemplateProcessorAddon _getProcessor()
	{

		if (Content.ContentProcessorType is not null && Content.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
		{
			return (ITfTemplateProcessorAddon)Activator.CreateInstance(Content.ContentProcessorType);

		}
		return null;

	}

	private void _initDynamicComponent()
	{
		_processor = _getProcessor();

		_dynamicComponentContext = new TfTemplateProcessorManageSettingsScreenRegionContext
		{
			SettingsJsonChanged = EventCallback.Factory.Create<string>(this, _settingsChanged),
			Template = Content with { Id = Content.Id },
		};
		_dynamicComponentScope = new TfScreenRegionScope(_processor.GetType(), null);
	}


}

