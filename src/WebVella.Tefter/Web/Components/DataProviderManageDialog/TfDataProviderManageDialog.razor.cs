namespace WebVella.Tefter.Web.Components.DataProviderManageDialog;
public partial class TfDataProviderManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProvider>
{
	[Inject]
	private DataProviderAdminUseCase UC { get; set; }
	[Parameter] public TucDataProvider Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private DynamicComponent typeSettingsComponent;
	private bool _isCreate = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		base.InitForm(UC.ProviderForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create data provider") : LOC("Manage data provider");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (_isCreate)
			{
				UC.ProviderForm = UC.ProviderForm with
				{
					ProviderType = UC.AllProviderTypes.First(),
				};
			}
			else
			{
				UC.ProviderForm = UC.ProviderForm with
				{
					Id = Content.Id,
					Name = Content.Name,
					SettingsJson = Content.SettingsJson,
				};
				if (Content.ProviderType is not null)
				{
					UC.ProviderForm.ProviderType = UC.AllProviderTypes.FirstOrDefault(x => x.Id == Content.ProviderType.Id);
				}
			}
			base.InitForm(UC.ProviderForm);
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);

			MessageStore.Clear();

			//Get dynamic settings component errors
			List<ValidationError> settingsErrors = new();
			if (UC.ProviderForm.ProviderType.SettingsComponentType is not null
				&& UC.ProviderForm.ProviderType.SettingsComponentType.GetInterface(nameof(ITfDataProviderSettings)) is not null)
			{
				settingsErrors = (typeSettingsComponent.Instance as ITfDataProviderSettings).Validate();
			}

			//Check form
			var isValid = EditContext.Validate();
			if (!isValid || settingsErrors.Count > 0) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			Result<TucDataProvider> submitResult;
			UC.ProviderForm.SettingsJson = (typeSettingsComponent.Instance as ITfDataProviderSettings).Value;
			if (_isCreate)
			{
				submitResult = UC.CreateDataProviderWithForm();
			}
			else
			{
				submitResult = UC.UpdateDataProviderWithForm();
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
		dict["DisplayMode"] = ComponentDisplayMode.Form;
		dict["Value"] = UC.ProviderForm.SettingsJson;
		return dict;
	}

}

