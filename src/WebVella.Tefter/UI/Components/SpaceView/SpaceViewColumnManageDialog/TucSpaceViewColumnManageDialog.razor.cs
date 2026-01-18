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

	private ITfSpaceViewColumnTypeAddon? _selectedColumnType = null;
	private List<ITfSpaceViewColumnTypeAddon> _availableColumnTypes = null!;
	private TucSpaceViewColumnManageDialogTab _activeTab = TucSpaceViewColumnManageDialogTab.General;
	private List<TfMenuItem> _menu = new();
	private TfSpaceViewColumn _form = new();
	private TfSpaceViewColumnOptionsMode _typeOptions = null!;
	private Dictionary<string, object> _contextViewData = new();
	private TfSpaceView _spaceView = new();
	private TfDataTable _sampleData = null!;
	private Dictionary<string, List<string>> _optionsDict = new();
	private List<string> _defaultFilterComparisonMethodOptions = new();

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
				Position = Content.Position
			};
			_selectedColumnType = TfMetaService.GetSpaceViewColumnType(_form.TypeId);
			_availableColumnTypes = TfMetaService.GetSpaceViewColumnTypesMeta();
		}
		else
		{
			Content = TfService.GetSpaceViewColumn(Content.Id);
			_form = Content with { Id = Content.Id };
			_selectedColumnType = TfMetaService.GetSpaceViewColumnType(_form.TypeId);
			if (_selectedColumnType == null)
				_availableColumnTypes = TfMetaService.GetSpaceViewColumnTypesMeta();
			else
				_availableColumnTypes = TfService.GetCompatibleViewColumnTypesMeta(_form);
		}

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
		_typeOptions = new TfSpaceViewColumnOptionsMode(_contextViewData)
		{
			TfService = TfService,
			ServiceProvider = ServiceProvider,
			ViewColumn = _form,
			ValidationErrors = new(),
			SettingsChanged = EventCallback.Factory.Create<string>(this, _columnTypeSettingsChangedHandler),
			DataMappingChanged = EventCallback.Factory.Create<Tuple<string, string?>>(this, _dataMappingValueChanged),
		};
		_initDefaultFilterTypeOption();
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
			var optionValErrors = _selectedColumnType.ValidateTypeOptions(_typeOptions);
			foreach (var valError in optionValErrors)
			{
				MessageStore.Add(EditContext.Field(nameof(_form.TypeOptionsJson)), valError.Message);
			}

			foreach (var definition in _selectedColumnType.DataMappingDefinitions)
			{
				if (!_form.DataMapping.ContainsKey(definition.Alias) ||
				    String.IsNullOrWhiteSpace(_form.DataMapping[definition.Alias]))
					MessageStore.Add(EditContext.Field(nameof(_form.DataMapping)), "required");
			}

			_initMenu();
			await InvokeAsync(StateHasChanged);
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

			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfSpaceViewUpdatedEventPayload(_spaceView));
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
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfSpaceViewUpdatedEventPayload(_spaceView));
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
				if (column.Origin == TfDataColumnOriginType.System
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
		_initDefaultFilterTypeOption();
	}

	private void _initDefaultFilterTypeOption()
	{
		_defaultFilterComparisonMethodOptions.Clear();

		foreach (var key in _form.DataMapping.Keys)
		{
			var dbColumnName = _form.DataMapping[key];
			if (String.IsNullOrWhiteSpace(dbColumnName)) continue;
			var dbColumn = _sampleData.Columns[dbColumnName];
			if (dbColumn is null) continue;
			switch (dbColumn.DbType)
			{
				case TfDatabaseColumnType.ShortInteger:
				case TfDatabaseColumnType.Integer:
				case TfDatabaseColumnType.LongInteger:
				case TfDatabaseColumnType.Number:
					foreach (TfFilterNumericComparisonMethod item in Enum.GetValues<TfFilterNumericComparisonMethod>())
						_defaultFilterComparisonMethodOptions.Add(item.ToDescriptionString());
					break;
				case TfDatabaseColumnType.Boolean:
					foreach (TfFilterBooleanComparisonMethod item in Enum.GetValues<TfFilterBooleanComparisonMethod>())
						_defaultFilterComparisonMethodOptions.Add(item.ToDescriptionString());
					break;
				case TfDatabaseColumnType.DateOnly:
				case TfDatabaseColumnType.DateTime:
					foreach (TfFilterDateTimeComparisonMethod item in
					         Enum.GetValues<TfFilterDateTimeComparisonMethod>())
						_defaultFilterComparisonMethodOptions.Add(item.ToDescriptionString());
					break;
				case TfDatabaseColumnType.ShortText:
				case TfDatabaseColumnType.Text:
					foreach (TfFilterTextComparisonMethod item in Enum.GetValues<TfFilterTextComparisonMethod>())
						_defaultFilterComparisonMethodOptions.Add(item.ToDescriptionString());
					break;
				case TfDatabaseColumnType.Guid:
					foreach (TfFilterGuidComparisonMethod item in Enum.GetValues<TfFilterGuidComparisonMethod>())
						_defaultFilterComparisonMethodOptions.Add(item.ToDescriptionString());
					break;
			}
		}

		if (!String.IsNullOrWhiteSpace(_form.Settings.DefaultComparisonMethodDescription) &&
		    !_defaultFilterComparisonMethodOptions.Contains(_form.Settings.DefaultComparisonMethodDescription))
			_form.Settings.DefaultComparisonMethodDescription = null;
	}

	private async Task _columnTypeSettingsChangedHandler(string value)
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
		var dataMappingErrors = EditContext.GetValidationMessages(EditContext.Field(nameof(_form.DataMapping)))
			.ToList();
		var allErrors = EditContext.GetValidationMessages().ToList();
		var tabValidationDict = new Dictionary<TucSpaceViewColumnManageDialogTab, bool>
		{
			[TucSpaceViewColumnManageDialogTab.General] =
				(allErrors.Count() - typeOptionErrors.Count() - dataMappingErrors.Count()) > 0,
			[TucSpaceViewColumnManageDialogTab.ColumnType] = typeOptionErrors.Any() || dataMappingErrors.Any()
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