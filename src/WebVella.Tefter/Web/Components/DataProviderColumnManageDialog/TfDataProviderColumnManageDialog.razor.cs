namespace WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;
public partial class TfDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<Tuple<TfDataProviderColumn,TfDataProvider>>
{
	[Parameter]
	public Tuple<TfDataProviderColumn, TfDataProvider> Content { get; set; }

	[CascadingParameter]
	public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private TfDataProviderColumn _form = new();
	private TfDataProvider _provider = new();
	private List<ITfDataProviderType> _allTypes = new();
	
	private string value1 = "1";

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if(Content is null) throw new Exception("Content is null");
		if(Content.Item1 is null ) throw new Exception("DataProviderColumn not provided");
		if(Content.Item2 is null ) throw new Exception("DataProvider not provided");
		if(Content.Item2.SupportedSourceDataTypes is null 
		|| !Content.Item2.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");
		if (Content.Item1.Id == Guid.Empty)
		{
			_title = LOC("Create column");
			_btnText = LOC("Create");
			_iconBtn = new Icons.Regular.Size20.Add();
		}
		else
		{
			_title = LOC("Manage column");
			_btnText = LOC("Save");
			_iconBtn = new Icons.Regular.Size20.Save();
		}
		base.InitForm(_form);
		_provider = Content.Item2;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
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
			_allTypes = typesResult.Value.ToList();
			if (!_allTypes.Any()) throw new Exception("No Data provider types found in application");
			//Setup form
			if (Content.Item1.Id == Guid.Empty)
			{
				_form = new TfDataProviderColumn()
				{
					DataProviderId = Content.Item2.Id,
					SourceType = Content.Item2.SupportedSourceDataTypes.First()
				};
			}
			else
			{
				_form = new TfDataProviderColumn()
				{
					Id = Content.Item1.Id,
					//Name = Content.Name,
					//ProviderType = Content.ProviderType,
					//CompositeKeyPrefix = Content.CompositeKeyPrefix,
					//SettingsJson = Content.SettingsJson
				};
			}
			base.InitForm(_form);
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
			//List<ValidationError> settingsErrors = new();
			//if (_form.ProviderType.SettingsComponentType is not null
			//	&& _form.ProviderType.SettingsComponentType.GetInterface(nameof(ITfDataProviderSettings)) is not null)
			//{
			//	settingsErrors = (typeSettingsComponent.Instance as ITfDataProviderSettings).Validate();
			//}

			////Check form
			//var isValid = EditContext.Validate();
			//if (!isValid || settingsErrors.Count > 0) return;

			//_isSubmitting = true;
			//await InvokeAsync(StateHasChanged);
			//Result<TfDataProvider> submitResult;
			//_form.SettingsJson = (typeSettingsComponent.Instance as ITfDataProviderSettings).Value;
			//if (_form.Id == Guid.Empty)
			//{
			//	submitResult = DataProviderManager.CreateDataProvider(_form);
			//}
			//else
			//{
			//	submitResult = DataProviderManager.UpdateDataProvider(_form);
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
		//dict["DisplayMode"] = ComponentDisplayMode.Form;
		//dict["Value"] = _form.SettingsJson;
		return dict;
	}

}

