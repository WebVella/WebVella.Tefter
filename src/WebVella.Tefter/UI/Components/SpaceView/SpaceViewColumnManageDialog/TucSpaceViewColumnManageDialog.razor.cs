namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceViewColumn?>
{
	[Parameter] public TfSpaceViewColumn? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private bool _isCreate = false;
	//NOTE: this changes the Items of the component type select
	//there is a bug and the component needs to be rerendered when both value and items ara changed
	private bool _renderComponentTypeSelect = false;
	private string _activeTab = "data";
	private TfSpaceViewColumn _form = new();
	private TfSpaceView _spaceView = new();
	private TfDataset _spaceData = new();
	private TfDataProvider _provider = new();
	private List<string> _options = new();
	private ReadOnlyCollection<TfSpaceViewColumnTypeAddonMeta> _availableColumnTypes = null!;
	private TfSpaceViewColumnTypeAddonMeta? _selectedColumnType = null;
	private List<ITfSpaceViewColumnComponentAddon> _selectedColumnTypeComponents = new();
	private ITfSpaceViewColumnComponentAddon? _selectedColumnComponent = null;
	private ITfSpaceViewColumnComponentAddon? _selectedEditColumnComponent = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		if (_isCreate)
		{
			_form = _form with
			{
				Id = Guid.NewGuid(),
				QueryName = NavigatorExt.GenerateQueryName(),
				SpaceViewId = Content.SpaceViewId,
				TypeId = new Guid(TfTextViewColumnType.ID),
				ComponentId = new Guid(TucTextDisplayColumnComponent.ID),
			};
		}
		else
		{
			Content = TfService.GetSpaceViewColumn(Content.Id);
			_form = Content with { Id = Content.Id };
		}
		if (_form.ComponentId == Guid.Empty)
			_form.ComponentId = new Guid(TucTextDisplayColumnComponent.ID);

		_availableColumnTypes = TfMetaService.GetSpaceViewColumnTypesMeta();

		_selectComponentType(_form.TypeId);

		base.InitForm(_form);
		_renderComponentTypeSelect = true;
		_spaceView = TfService.GetSpaceView(_form.SpaceViewId);
		_spaceData = TfService.GetDataset(_spaceView.DatasetId);
		_options = new();
		if (_spaceData is not null)
		{
			if (_spaceData.Columns.Count > 0 || _spaceData.Identities.Count > 0)
			{
				if (_spaceData.Columns.Count > 0)
					_options.AddRange(_spaceData.Columns);

				foreach (var identity in _spaceData.Identities)
				{
					foreach (var column in identity.Columns)
					{
						_options.Add($"{identity.DataIdentity}.{column}");
					}
				}
			}

			else
			{
				//This space dataset uses all the columns from the data provider
				var dataProvider = TfService.GetDataProvider(_spaceData.DataProviderId);
				if (dataProvider is not null)
				{
					_options.AddRange(dataProvider.Columns.Select(x => (x.DbName ?? String.Empty)));
					_options.AddRange(dataProvider.SharedColumns.Select(x => (x.DbName ?? String.Empty)));
				}
			}
		}
	}

	private void _selectComponentType(Guid typeId)
	{
		_selectedColumnType = TfMetaService.GetSpaceViewColumnType(typeId);
		_selectedColumnTypeComponents = new();

		if (_selectedColumnType is not null)
			_selectedColumnTypeComponents = TfMetaService.GetSpaceViewColumnTypeSupportedComponents(_selectedColumnType.Instance.AddonId);

		if (_selectedColumnTypeComponents.Count > 0)
		{
			//Display
			if (_form.ComponentId != Guid.Empty)
			{
				_selectedColumnComponent = _selectedColumnTypeComponents.FirstOrDefault(x => x.AddonId == _form.ComponentId);
			}
			if (_selectedColumnComponent is null)
			{
				if (_selectedColumnType!.Instance.DefaultDisplayComponentId is not null)

					_selectedColumnComponent = _selectedColumnTypeComponents.FirstOrDefault(x => x.AddonId == _selectedColumnType!.Instance.DefaultDisplayComponentId);
				else
					_selectedColumnComponent = _selectedColumnTypeComponents[0];
			}
			_form.ComponentId = _selectedColumnComponent!.AddonId;

			//Edit
			if (_form.EditComponentId != Guid.Empty)
			{
				_selectedEditColumnComponent = _selectedColumnTypeComponents.FirstOrDefault(x => x.AddonId == _form.EditComponentId);
			}
			if (_selectedEditColumnComponent is null)
			{
				if (_selectedColumnType!.Instance.DefaultEditComponentId is not null)
					_selectedEditColumnComponent = _selectedColumnTypeComponents.FirstOrDefault(x => x.AddonId == _selectedColumnType!.Instance.DefaultEditComponentId);
				else
					_selectedEditColumnComponent = _selectedColumnTypeComponents[0];
			}
			_form.EditComponentId = _selectedEditColumnComponent!.AddonId;
		}
		else
		{
			_selectedColumnComponent = null;
			_selectedEditColumnComponent = null;
			_form.ComponentId = Guid.Empty;
			_form.EditComponentId = Guid.Empty;
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

	private async Task _columnTypeChangeHandler(TfSpaceViewColumnTypeAddonMeta columnType)
	{
		_renderComponentTypeSelect = false;
		_selectedColumnType = null;
		_selectedColumnComponent = null;
		_form.TypeId = Guid.Empty;
		if (columnType is not null)
		{
			_selectedColumnType = columnType;
			_form.TypeId = columnType.Instance.AddonId;
		}

		if (_selectedColumnType is null)
		{
			_selectedColumnType = TfMetaService.GetSpaceViewColumnType(new Guid(TfTextViewColumnType.ID));
			_form.TypeId = _selectedColumnType.Instance.AddonId;
		}
		_selectComponentType(_form.TypeId);
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_renderComponentTypeSelect = true;
		await InvokeAsync(StateHasChanged);
	}
	private void _columnComponentChangeHandler(ITfSpaceViewColumnComponentAddon componentType)
	{
		_selectedColumnComponent = null;
		_form.ComponentId = Guid.Empty;
		if (_selectedColumnType is not null)
		{
			if (componentType is not null)
			{
				_selectedColumnComponent = componentType;
				_form.ComponentId = componentType.AddonId;
			}

			if (_selectedColumnComponent is null)
			{
				Guid defaultCompId = _selectedColumnType.Instance.DefaultDisplayComponentId is not null ? _selectedColumnType.Instance.DefaultDisplayComponentId.Value
					: new Guid(TucTextDisplayColumnComponent.ID);
				_selectedColumnComponent = TfMetaService.GetSpaceViewColumnComponent(defaultCompId);
				_form.ComponentId = _selectedColumnComponent.AddonId;
			}

		}
	}

	private void _columnEditComponentChangeHandler(ITfSpaceViewColumnComponentAddon componentType)
	{
		_selectedEditColumnComponent = null;
		_form.EditComponentId = Guid.Empty;
		if (_selectedColumnType is not null)
		{
			if (componentType is not null)
			{
				_selectedColumnComponent = componentType;
				_form.EditComponentId = componentType.AddonId;
			}

			if (_selectedColumnComponent is null)
			{
				Guid defaultCompId = _selectedColumnType.Instance.DefaultEditComponentId is not null ? _selectedColumnType.Instance.DefaultEditComponentId.Value
					: new Guid(TucTextDisplayColumnComponent.ID);
				_selectedEditColumnComponent = TfMetaService.GetSpaceViewColumnComponent(defaultCompId);
				_form.EditComponentId = _selectedColumnComponent.AddonId;
			}

		}
	}

	private string? _getDataMappingValue(string alias)
	{
		if (_form.DataMapping.ContainsKey(alias))
			return _form.DataMapping[alias];

		return null;
	}

	private void _dataMappingValueChanged(Tuple<string, string> valueAlias)
	{
		if (valueAlias is null || _selectedColumnType is null) return;

		//fix datamapping object based on the latest requirements
		var dataMapping = new Dictionary<string, string>();
		dataMapping[valueAlias.Item1] = valueAlias.Item2;
		foreach (var item in _selectedColumnType.Instance.DataMapping)
		{
			if (item.Alias == valueAlias.Item1) continue; //already added above
			dataMapping[item.Alias] = null;
			if (_form.DataMapping.ContainsKey(item.Alias))
				dataMapping[item.Alias] = _form.DataMapping[item.Alias];
		}
		_form.DataMapping = dataMapping;
	}

	private Dictionary<string, object> _getColumnComponentContext()
	{
		var componentData = new Dictionary<string, object>();

		var contextData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegionContext(contextData)
		{
			Mode = TfComponentPresentationMode.Options,
			ComponentOptionsJson = _form.ComponentOptionsJson,
			DataMapping = _form.DataMapping,
			DataTable = null,
			RowId = Guid.Empty,
			QueryName = _form.QueryName,
			SpaceViewId = _form.SpaceViewId,
			SpaceViewColumnId = _form.Id,
			EditContext = EditContext,
			ValidationMessageStore = MessageStore
		};
		componentData[TfConstants.SPACE_VIEW_COMPONENT_OPTIONS_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<string>(this, _customOptionsChangedHandler);
		componentData[TfConstants.SPACE_VIEW_COMPONENT_DATA_MAPPING_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<Tuple<string, string>>(this, _dataMappingValueChanged);
		return componentData;
	}

	private Dictionary<string, object> _getColumnEditComponentContext()
	{
		var componentData = new Dictionary<string, object>();

		var contextData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegionContext(contextData)
		{
			Mode = TfComponentPresentationMode.Options,
			ComponentOptionsJson = _form.EditComponentOptionsJson,
			DataMapping = _form.DataMapping,
			DataTable = null,
			RowId = Guid.Empty,
			QueryName = _form.QueryName,
			SpaceViewId = _form.SpaceViewId,
			SpaceViewColumnId = _form.Id,
			EditContext = EditContext,
			ValidationMessageStore = MessageStore
		};
		componentData[TfConstants.SPACE_VIEW_COMPONENT_OPTIONS_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<string>(this, _customEditOptionsChangedHandler);
		componentData[TfConstants.SPACE_VIEW_COMPONENT_DATA_MAPPING_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<Tuple<string, string>>(this, _dataMappingValueChanged);
		return componentData;
	}

	private async Task _customOptionsChangedHandler(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) _form.ComponentOptionsJson = null;

		if (!(value.StartsWith("{") && value.StartsWith("{"))
		|| (value.StartsWith("[") && value.StartsWith("]")))
		{
			ToastService.ShowError("custom options value needs to be json");
			return;
		}

		_form.ComponentOptionsJson = value;
		await InvokeAsync(StateHasChanged);
	}


	private async Task _customEditOptionsChangedHandler(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) _form.EditComponentOptionsJson = null;

		if (!(value.StartsWith("{") && value.StartsWith("{"))
		|| (value.StartsWith("[") && value.StartsWith("]")))
		{
			ToastService.ShowError("custom options value needs to be json");
			return;
		}

		_form.EditComponentOptionsJson = value;
		await InvokeAsync(StateHasChanged);
	}

	private List<Option<string>> _getTypeListAsOptions(List<Type> typeList)
	{
		var result = new List<Option<string>>();
		if (typeList is null) return result;

		foreach (var item in typeList)
		{
			result.Add(new Option<string> { Text = item.ToDescriptionString(), Value = item.Name });
		}

		return result;

	}

	private void _titleChanged(string title)
	{
		_form.Title = title;
	}
}
