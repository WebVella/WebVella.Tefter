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

	private DynamicComponent typeSettingsComponent;
	private bool _isCreate = false;
	private TucDataProviderForm _form = new();
	private TfDataProviderSettingsComponentContext _setttingsContext = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
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
		_setttingsContext = new TfDataProviderSettingsComponentContext{
			SettingsJson = _form.SettingsJson,
			SettingsJsonChanged = EventCallback.Factory.Create<string>(this, _settingsChanged),
		};
		base.InitForm(_form);

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{

			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			MessageStore.Clear();

			//Get dynamic settings component errors
			List<ValidationError> settingsErrors = new();
			if (_setttingsContext.Validate is not null)
			{
				settingsErrors = _setttingsContext.Validate();
			}

			//Check form
			var isValid = EditContext.Validate();
			if (!isValid || settingsErrors.Count > 0) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			Result<TucDataProvider> submitResult;
			if (_isCreate)
			{
				submitResult = UC.CreateDataProviderWithForm(_form);
			}
			else
			{
				submitResult = UC.UpdateDataProviderWithForm(_form);
			}

			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				await Dialog.CloseAsync(submitResult.Value);
			}
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

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = TfComponentMode.Update;
		dict["Context"] = _setttingsContext;
		return dict;
	}
	private void _settingsChanged(string json)
	{
		_form.SettingsJson = json;
	}

}

