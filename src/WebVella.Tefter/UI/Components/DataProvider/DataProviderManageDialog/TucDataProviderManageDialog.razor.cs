namespace WebVella.Tefter.UI.Components;

public partial class TucDataProviderManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProvider?>
{
	[Parameter] public TfDataProvider? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;

	private bool _isCreate = false;
	private TfCreateDataProvider _form = new();

	private TfDataProviderManageSettingsScreenRegionContext _dynamicComponentContext = null!;
	private TfScreenRegionScope? _dynamicComponentScope = null;
	private ReadOnlyCollection<ITfDataProviderAddon> _providerTypes = null!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_providerTypes = TfMetaService.GetDataProviderTypes();
		if (_providerTypes.Count == 0)
			throw new Exception("No provider types are found");
		_initForm();
		_initDynamicComponent();
	}
	private void _initForm()
	{
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create data provider") : LOC("Manage data provider");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		if (_isCreate)
		{
			_form = new TfCreateDataProvider
			{
				ProviderType = _providerTypes.First(),
				AutoInitialize = true
			};
		}
		else
		{
			_form = new TfCreateDataProvider
			{
				Id = Content.Id,
				Index = Content.Index,
				Name = Content.Name,
				ProviderType = Content.ProviderType,
				SettingsJson = Content.SettingsJson,
				SynchPrimaryKeyColumns = Content.SynchPrimaryKeyColumns.ToList(),
				SynchScheduleMinutes = Content.SynchScheduleMinutes,
				SynchScheduleEnabled = Content.SynchScheduleEnabled,
				AutoInitialize = false
			};
		}
		base.InitForm(_form);
	}

	private void _initDynamicComponent()
	{
		_dynamicComponentContext = new TfDataProviderManageSettingsScreenRegionContext
		{
			SettingsJson = _form.SettingsJson,
			SettingsJsonChanged = EventCallback.Factory.Create<string>(this, _settingsChanged),
		};
		_dynamicComponentScope = new TfScreenRegionScope(_form.ProviderType.GetType(), null);
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			MessageStore.Clear();

			//Get dynamic settings component errors
			List<ValidationError> settingsErrors = new();
			if (_dynamicComponentContext.Validate is not null)
			{
				settingsErrors = _dynamicComponentContext.Validate();
			}

			//Check form
			var isValid = EditContext.Validate();
			if (!isValid || settingsErrors.Count > 0)
			{
				return;
			}

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			TfDataProvider provider;
			if (_isCreate)
			{
				provider = TfUIService.CreateDataProvider(new TfCreateDataProvider
				{
					Id = _form.Id,
					Index = -1,
					Name = _form.Name,
					ProviderType = _form.ProviderType,
					SettingsJson = _form.SettingsJson,
					SynchPrimaryKeyColumns = _form.SynchPrimaryKeyColumns,
					SynchScheduleEnabled = !_form.AutoInitialize ? false : _form.SynchScheduleEnabled,
					SynchScheduleMinutes = _form.SynchScheduleMinutes,
					AutoInitialize = _form.AutoInitialize
				});
			}
			else
			{
				provider = TfUIService.UpdateDataProvider(new TfUpdateDataProvider
				{
					Id = _form.Id,
					Name = _form.Name,
					SettingsJson = _form.SettingsJson,
					SynchPrimaryKeyColumns = _form.SynchPrimaryKeyColumns,
					SynchScheduleEnabled = _form.SynchScheduleEnabled,
					SynchScheduleMinutes = _form.SynchScheduleMinutes,
				});
			}
			await Dialog.CloseAsync(provider);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
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
		_form.SettingsJson = json;
		StateHasChanged();
	}

	private void _providerTypeChanged(ITfDataProviderAddon selection)
	{
		_form.ProviderType = selection;
		_initDynamicComponent();
	}

}

