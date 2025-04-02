namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderManageDialog.TfDataProviderManageDialog", "WebVella.Tefter")]
public partial class TfDataProviderManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProvider>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucDataProvider Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private bool _isCreate = false;
	private TucDataProviderForm _form = new();

	private TfDataProviderManageSettingsComponentContext _dynamicComponentContext = null;
	private TfRegionComponentScope _dynamicComponentScope = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initForm();
		_initDynamicComponent();
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		_initForm();
		_initDynamicComponent();
	}

	private void _initForm()
	{
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create data provider") : LOC("Manage data provider");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		if (_isCreate)
		{
			_form = _form with
			{
				ProviderType = TfAppState.Value.DataProviderTypes.FirstOrDefault(),
			};
		}
		else
		{
			_form = _form with
			{
				Id = Content.Id,
				Name = Content.Name,
				SettingsJson = Content.SettingsJson,
			};
			if (Content.ProviderType is not null)
			{
				_form.ProviderType = TfAppState.Value.DataProviderTypes.FirstOrDefault(x => x.Id == Content.ProviderType.Id);
			}
		}
		base.InitForm(_form);
	}

	private void _initDynamicComponent()
	{
		_dynamicComponentContext = new TfDataProviderManageSettingsComponentContext
		{
			SettingsJson = _form.SettingsJson,
			SettingsJsonChanged = EventCallback.Factory.Create<string>(this, _settingsChanged),
		};
		_dynamicComponentScope = new TfRegionComponentScope(_form.ProviderType.Model.GetType(), null);
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
			if (!isValid || settingsErrors.Count > 0) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TucDataProvider provider;
			if (_isCreate)
			{
				provider = UC.CreateDataProviderWithForm(_form);
			}
			else
			{
				provider = UC.UpdateDataProviderWithForm(_form);
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
	}

	private void _providerTypeChanged(TucDataProviderTypeInfo selection)
	{
		_form.ProviderType = selection;
		_initDynamicComponent();
	}

}

