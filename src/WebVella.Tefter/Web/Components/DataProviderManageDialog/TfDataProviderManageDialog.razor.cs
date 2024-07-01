namespace WebVella.Tefter.Web.Components.DataProviderManageDialog;
public partial class TfDataProviderManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProvider>
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Parameter] public TfDataProvider Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private IEnumerable<TucDatabaseColumnTypeInfo> _allTypes = Enumerable.Empty<TucDatabaseColumnTypeInfo>();
	private DynamicComponent typeSettingsComponent;
	private bool _isCreate = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		UC.OnInitialized(
			initForm:true,
			initMenu: false
			);
		base.InitForm(UC.Form);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create data provider") : LOC("Manage data provider");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();
		
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
		}
	}

	private async Task _loadDataAsync()
	{
		_isBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			//Init type options
			var typesResult = DataProviderManager.GetProviderTypes();
			if (!typesResult.IsSuccess) throw new Exception("Cannot load data provider types");
			//_allTypes = typesResult.Value.ToList().AsEnumerable<TucDatabaseColumnTypeInfo>();
			if (!_allTypes.Any()) throw new Exception("No Data provider types found in application");
			//Setup form
			//if (Content.Id == Guid.Empty)
			//{
			//	UC.Form = new TfDataProviderModel()
			//	{
			//		ProviderType = _allTypes[0]
			//	};
			//}
			//else
			//{
			//	UC.Form = new TfDataProviderModel()
			//	{
			//		Id = Content.Id,
			//		Name = Content.Name,
			//		ProviderType = Content.ProviderType,
			//		SettingsJson = Content.SettingsJson
			//	};
			//}
			base.InitForm(UC.Form);
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
		}
		finally
		{


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
			if (UC.Form.ProviderType.SettingsComponentType is not null
				&& UC.Form.ProviderType.SettingsComponentType.GetInterface(nameof(ITfDataProviderSettings)) is not null)
			{
				settingsErrors = (typeSettingsComponent.Instance as ITfDataProviderSettings).Validate();
			}

			//Check form
			var isValid = EditContext.Validate();
			if (!isValid || settingsErrors.Count > 0) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			Result<TfDataProvider> submitResult;
			UC.Form.SettingsJson = (typeSettingsComponent.Instance as ITfDataProviderSettings).Value;
			//if (_isCreate)
			//{
			//	submitResult = DataProviderManager.CreateDataProvider(UC.Form);
			//}
			//else
			//{
			//	submitResult = DataProviderManager.UpdateDataProvider(UC.Form);
			//}

			//ProcessFormSubmitResponse(submitResult);
			//if (submitResult.IsSuccess)
			//{
			//	await Dialog.CloseAsync(submitResult.Value);
			//}
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
		dict["Value"] = UC.Form.SettingsJson;
		return dict;
	}

}

