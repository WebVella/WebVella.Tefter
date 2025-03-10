namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateSettingsDialog.TfTemplateSettingsDialog", "WebVella.Tefter")]
public partial class TfTemplateSettingsDialog : TfBaseComponent, IDialogContentComponent<TucTemplate>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucTemplate Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private string _form = null;

	private ITfTemplateProcessor _processor = null;
	private TfTemplateProcessorManageSettingsComponentContext _dynamicComponentContext = null;
	private TfRegionComponentScope _dynamicComponentScope = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Update settings");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save");
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
			var template = UC.UpdateTemplateSettings(Content.Id, _form);
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

	private ITfTemplateProcessor _getProcessor()
	{

		if (Content.ContentProcessorType is not null && Content.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessor)) != null)
		{
			return (ITfTemplateProcessor)Activator.CreateInstance(Content.ContentProcessorType);

		}
		return null;

	}

	private void _initDynamicComponent()
	{
		_processor = _getProcessor();

		_dynamicComponentContext = new TfTemplateProcessorManageSettingsComponentContext
		{
			SettingsJsonChanged = EventCallback.Factory.Create<string>(this, _settingsChanged),
			Template = Content with { Id = Content.Id },
		};
		_dynamicComponentScope = new TfRegionComponentScope(_processor.GetType(),null);
	}


}

