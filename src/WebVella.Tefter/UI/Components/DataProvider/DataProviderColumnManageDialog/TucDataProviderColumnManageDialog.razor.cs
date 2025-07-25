﻿namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderColumnManageDialog.TfDataProviderColumnManageDialog", "WebVella.Tefter")]
public partial class TucDataProviderColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProviderColumn?>
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public TfDataProviderColumn? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private bool _isConnected = false;
	private Icon _iconBtn = default!;
	private TfDataProvider _provider = new();
	private TfDataProviderColumn _form = new();
	private Dictionary<TfDatabaseColumnType, List<string>> _providerColumnTypeToSourceTypes = new();
	private List<DatabaseColumnTypeInfo> _providerColumnTypeOptions = new();
	private List<TfDataProviderColumnSearchType> _searchTypes = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		//Init Content
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProvider not provided");

		//Init provider
		_provider = TfDataProviderUIService.GetDataProvider(Content.DataProviderId);
		if (_provider is null || _provider.SupportedSourceDataTypes is null
		|| !_provider.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");

		//Init form
		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}

		_providerColumnTypeOptions = TfDataProviderUIService.GetDatabaseColumnTypeInfosList()
			.Where(x => x.Type != TfDatabaseColumnType.AutoIncrement).ToList();
		_providerColumnTypeToSourceTypes = new();
		foreach (var sourceType in _provider.ProviderType.GetSupportedSourceDataTypes())
		{
			foreach (var providerType in _provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(sourceType))
			{
				if (!_providerColumnTypeToSourceTypes.ContainsKey(providerType))
					_providerColumnTypeToSourceTypes[providerType] = new();

				_providerColumnTypeToSourceTypes[providerType].Add(sourceType);
			}
		}
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		foreach (TfDataProviderColumnSearchType item in Enum.GetValues<TfDataProviderColumnSearchType>())
		{
			_searchTypes.Add(item);
		}
		_isConnected = !String.IsNullOrWhiteSpace(_form.SourceName);
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
			if (_isCreate)
			{
				_form = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = _provider.Id,
					CreatedOn = DateTime.Now
				};
				if(_providerColumnTypeToSourceTypes[_form.DbType].Count > 0){ 
					_form.SourceType = _providerColumnTypeToSourceTypes[_form.DbType][0];
				}
			}
			else
			{
				_form = new TfDataProviderColumn
				{
					Id = Content!.Id,
					AutoDefaultValue = Content.AutoDefaultValue,
					CreatedOn = Content.CreatedOn,
					DataProviderId = Content.DataProviderId,
					DbName = Content.DbName,
					DbType = Content.DbType,
					PreferredSearchType = Content.PreferredSearchType,
					DefaultValue = Content.DefaultValue,
					IncludeInTableSearch = Content.IncludeInTableSearch,
					IsNullable = Content.IsNullable,
					IsSearchable = Content.IsSearchable,
					IsSortable = Content.IsSortable,
					IsUnique = Content.IsUnique,
					SourceName = Content.SourceName,
					SourceType = Content.SourceType,
				};
				_isConnected = !String.IsNullOrWhiteSpace(_form.SourceName);
			}
			base.InitForm(_form);
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
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

			var errors = new List<ValidationError>();

			if (_isConnected && String.IsNullOrWhiteSpace(_form.SourceName))
			{
				errors.Add(new ValidationError(nameof(_form.SourceName), LOC("required if attached")));
			}

			foreach (var item in errors)
			{
				MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
			}

			////Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			if (!_isConnected)
				_form.SourceName = null;

			if (!_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue))
			{
				_form.DefaultValue = String.Empty;
			}
			else if (_form.IsNullable && String.IsNullOrWhiteSpace(_form.DefaultValue))
			{
				_form.DefaultValue = null;
			}
			var submit = new TfDataProviderColumn
			{
				Id = _form.Id,
				AutoDefaultValue = _form.AutoDefaultValue,
				CreatedOn = _form.CreatedOn,
				DataProviderId = _form.DataProviderId,
				DbName = _form.DbName,
				DbType = _form.DbType,
				PreferredSearchType = _form.PreferredSearchType,
				DefaultValue = _form.DefaultValue,
				IncludeInTableSearch = _form.IncludeInTableSearch,
				IsNullable = _form.IsNullable,
				IsSearchable = _form.IsSearchable,
				IsSortable = _form.IsSortable,
				IsUnique = _form.IsUnique,
				SourceName = _form.SourceName,
				SourceType = _form.SourceType,
			};
			submit.FixPrefix(_provider.ColumnPrefix);
			if (_isCreate)
			{
				_provider = TfDataProviderUIService.CreateDataProviderColumn(submit);
				ToastService.ShowSuccess(LOC("Data provider column was created successfully"));
			}
			else
			{
				_provider = TfDataProviderUIService.UpdateDataProviderColumn(submit);
				ToastService.ShowSuccess(LOC("Data provider column was updated successfully"));
			}

			await Dialog.CloseAsync(_provider);
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
	private bool _providerTypeSupportsAutogen()
	{
		var dbInfo = _providerColumnTypeOptions.FirstOrDefault(x => x.Type == _form.DbType);
		if (dbInfo is null) return false;
		return dbInfo.SupportAutoDefaultValue;
	}
}
