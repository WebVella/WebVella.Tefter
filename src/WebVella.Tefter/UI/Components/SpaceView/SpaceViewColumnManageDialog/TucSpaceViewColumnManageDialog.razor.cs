namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceViewColumn?>
{
	[Parameter] public TfSpaceViewColumn? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;

	private bool _isCreate = false;

	private ITfSpaceViewColumnTypeAddon _selectedColumnType = null!;
	private ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> _availableColumnTypes = null!;
	private TucSpaceViewColumnManageDialogTab _activeTab = TucSpaceViewColumnManageDialogTab.General;
	private List<TfMenuItem> _menu = new();
	private TfSpaceViewColumn _form = new();
	private TfSpaceViewColumnOptionsModeContext _typeOptionsContext = new();

	private TfSpaceView _spaceView = new();
	private TfDataTable _sampleData = null!;
	private Dictionary<string,List<string>> _optionsDict = new();

	protected override void OnInitialized()
	{
		//Init General
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;

		//Init form
		if (_isCreate)
		{
			_form = _form with
			{
				Id = Guid.NewGuid(),
				QueryName = NavigatorExt.GenerateQueryName(),
				SpaceViewId = Content.SpaceViewId,
				TypeId = new Guid(TfTextViewColumnType.ID),
			};
		}
		else
		{
			Content = TfService.GetSpaceViewColumn(Content.Id);
			_form = Content with { Id = Content.Id };
		}

		_availableColumnTypes = TfMetaService.GetSpaceViewColumnTypesMeta();
		_selectedColumnType = _availableColumnTypes.FirstOrDefault(x => x.AddonId == _form.TypeId)
		                      ?? _availableColumnTypes.First();
		InitForm(_form);

		//Init data mapping options
		_spaceView = TfService.GetSpaceView(_form.SpaceViewId)!;
		var dataset = TfService.GetDataset(_spaceView.DatasetId);
		if (dataset is null)
			throw new Exception("This View has no dataset selected");

		_sampleData = TfService.QueryDataset(dataset.Id, page: 1, pageSize: 1);

		_initDataMappingOptions();
		//Init Menu and context
		_initMenu();
		_typeOptionsContext = new TfSpaceViewColumnOptionsModeContext()
		{
			TfService = TfService,
			ViewColumn = _form,
			ValidationErrors = new(),
			SettingsChanged = EventCallback.Factory.Create<string>(this, _customOptionsChangedHandler)
		};
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
			var optionValErrors = _selectedColumnType.ValidateTypeOptions(_typeOptionsContext);
			foreach (var valError in optionValErrors)
			{
				MessageStore.Add(EditContext.Field(nameof(_form.TypeOptionsJson)), valError.Message);
			}

			_initMenu();
			if (!EditContext.Validate()) return;
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new List<TfSpaceViewColumn>();
			if (_isCreate)
			{
				await TfService.CreateSpaceViewColumn(_form);
			}
			else
			{
				await TfService.UpdateSpaceViewColumn(_form);
			}

			await Dialog.CloseAsync(result);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
			_initMenu();
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _delete()
	{
		if (_isSubmitting) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this column deleted?")))
			return;
		try
		{
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			await TfService.DeleteSpaceViewColumn(_form.Id);
			await Dialog.CloseAsync((TfSpaceViewColumn?)null);
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

	private void _columnTypeChangeHandler(ITfSpaceViewColumnTypeAddon columnType)
	{
		_selectedColumnType = columnType;
		_form.TypeId = columnType.AddonId;
		_initDataMappingOptions();
	}

	private void _initDataMappingOptions()
	{
		_optionsDict = new();

		foreach (var definition in _selectedColumnType.DataMappingDefinitions)
		{
			_optionsDict[definition.Alias] = new();
			foreach (var column in _sampleData.Columns)
			{
				if(column.Origin == TfDataColumnOriginType.System
				   || column.Origin == TfDataColumnOriginType.Identity)
					continue;
				if (!definition.SupportedDatabaseColumnTypes.Contains(column.DbType))
					continue;
				
				_optionsDict[definition.Alias].Add(column.Name);
			}			
		}


	}

	private string? _getDataMappingValue(string alias)
	{
		if (_form.DataMapping.TryGetValue(alias, out string? value))
			return value;

		return null;
	}

	private void _dataMappingValueChanged(Tuple<string, string?> valueAlias)
	{
		//fix data mapping object based on the latest requirements
		var dataMapping = new Dictionary<string, string?> { [valueAlias.Item1] = valueAlias.Item2 };
		foreach (var item in _selectedColumnType.DataMappingDefinitions)
		{
			if (item.Alias == valueAlias.Item1) continue; //already added above
			dataMapping[item.Alias] = null;
			if (_form.DataMapping.TryGetValue(item.Alias, out string? value))
				dataMapping[item.Alias] = value;
		}

		_form.DataMapping = dataMapping;
	}

	private async Task _customOptionsChangedHandler(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) _form.TypeOptionsJson = null;

		if (!(value.StartsWith("{") && value.StartsWith("{"))
		    || (value.StartsWith("[") && value.StartsWith("]")))
		{
			ToastService.ShowError("custom options value needs to be json");
			return;
		}

		_form.TypeOptionsJson = value;
		await InvokeAsync(StateHasChanged);
	}

	private void _initMenu()
	{
		_menu = new();

		//Validation
		var typeOptionErrors = EditContext.GetValidationMessages(EditContext.Field(nameof(_form.TypeOptionsJson)))
			.ToList();
		var allErrors = EditContext.GetValidationMessages().ToList();
		var tabValidationDict = new Dictionary<TucSpaceViewColumnManageDialogTab, bool>
		{
			[TucSpaceViewColumnManageDialogTab.General] = (allErrors.Count() - typeOptionErrors.Count()) > 0,
			[TucSpaceViewColumnManageDialogTab.ColumnType] = typeOptionErrors.Any()
		};

		foreach (var tab in Enum.GetValues<TucSpaceViewColumnManageDialogTab>())
		{
			var tabId = ((int)tab).ToString();
			_menu.Add(new()
			{
				Id = tabId,
				Text = tab.ToDescriptionString(),
				OnClick = EventCallback.Factory.Create(this, () => _tabChanged(tab)),
				Selected = tabId == ((int)_activeTab).ToString(),
				BadgeContent = tabValidationDict[tab]
					? builder =>
					{
						builder.OpenComponent<FluentIcon<Icon>>(0);
						builder.AddAttribute(1, "Value",
							TfConstants.GetIcon("ErrorCircleRegular")!.WithColor(Color.Error));
						builder.CloseComponent();
					}
					: null
			});
		}
	}

	private void _tabChanged(TucSpaceViewColumnManageDialogTab tab)
	{
		_activeTab = tab;
		_initMenu();
		StateHasChanged();
	}

	private enum TucSpaceViewColumnManageDialogTab
	{
		[Description("General")] General = 0,
		[Description("Column Type")] ColumnType = 1,
	}
}