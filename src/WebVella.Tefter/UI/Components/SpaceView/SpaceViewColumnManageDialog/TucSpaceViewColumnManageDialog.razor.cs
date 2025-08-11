namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewColumnManageDialog.TucSpaceViewColumnManageDialog", "WebVella.Tefter")]
public partial class TucSpaceViewColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceViewColumn?>
{
	[Inject] public ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public TfSpaceViewColumn? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;
	//NOTE: this changes the Items of the component type select
	//there is a bug and the component needs to be rerendered when both value and items ara changed
	private bool _renderComponentTypeSelect = false;
	private string _activeTab = "data";
	private TfSpaceViewColumn _form = new();
	private TfSpaceView _spaceView = new();
	private TfSpaceData _spaceData = new();
	private TfDataProvider _provider = new();
	private List<string> _options = new();
	private ReadOnlyCollection<TfSpaceViewColumnTypeAddonMeta> _availableColumnTypes = default!;
	private TfSpaceViewColumnTypeAddonMeta? _selectedColumnType = null;
	private List<ITfSpaceViewColumnComponentAddon> _selectedColumnTypeComponents = new();
	private ITfSpaceViewColumnComponentAddon? _selectedColumnComponent = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
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

			_form = Content with { Id = Content.Id };
		}
		if (_form.ComponentId == Guid.Empty)
			_form.ComponentId = new Guid(TucTextDisplayColumnComponent.ID);

		_availableColumnTypes = TfMetaUIService.GetSpaceViewColumnTypes();

		_selectComponentType(_form.TypeId);

		base.InitForm(_form);
		_renderComponentTypeSelect = true;
		_spaceView = TfSpaceViewUIService.GetSpaceView(_form.SpaceViewId);
		_spaceData = TfSpaceDataUIService.GetSpaceData(_spaceView.SpaceDataId);
		if (_spaceData is not null)
		{
			if (_spaceData.Columns.Count > 0)
				_options = _spaceData.Columns;
			else
			{
				//This space dataset uses all the columns from the data provider
				var dataProvider = TfDataProviderUIService.GetDataProvider(_spaceData.DataProviderId);
				if (dataProvider is not null)
				{
					_options.AddRange(dataProvider.Columns.Select(x => (x.DbName ?? String.Empty)));
				}
			}
		}
	}

	private void _selectComponentType(Guid typeId)
	{
		_selectedColumnType = TfSpaceViewUIService.GetSpaceViewColumnTypeById(typeId);
		if (_selectedColumnType is not null)
			_selectedColumnTypeComponents = TfSpaceViewUIService.GetSpaceViewColumnTypeSupportedComponents(_selectedColumnType.Instance.AddonId);
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
				TfSpaceViewUIService.CreateSpaceViewColumn(_form);
			}
			else
			{
				TfSpaceViewUIService.UpdateSpaceViewColumn(_form);
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
		_form.TypeId = Guid.Empty;
		if (columnType is not null)
		{
			_selectedColumnType = columnType;
			_form.TypeId = columnType.Instance.AddonId;
		}

		if (_selectedColumnType is null)
		{
			_selectedColumnType = TfSpaceViewUIService.GetSpaceViewColumnTypeById(new Guid(TfTextViewColumnType.ID));
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
				Guid defaultCompId = _selectedColumnType.Instance.DefaultComponentId is not null ? _selectedColumnType.Instance.DefaultComponentId.Value
					: new Guid(TucTextDisplayColumnComponent.ID);
				_selectedColumnComponent = TfSpaceViewUIService.GetSpaceViewColumnComponentById(defaultCompId);
				_form.ComponentId = _selectedColumnComponent.AddonId;
			}
		}
	}

	private string? _getDataMappingValue(string alias)
	{
		if (_form.DataMapping.ContainsKey(alias))
			return _form.DataMapping[alias];

		return null;
	}

	private void _dataMappingValueChanged(string value, string alias)
	{
		_form.DataMapping[alias] = value;
	}

	private Dictionary<string, object> _getColumnComponentContext()
	{
		var componentData = new Dictionary<string, object>();

		var contextData = new Dictionary<string,object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegionContext(contextData)
		{
			Mode = TfComponentPresentationMode.Options,
			ComponentOptionsJson = _form.ComponentOptionsJson,
			DataMapping = _form.DataMapping,
			DataTable = null,
			RowIndex = -1,
			QueryName = _form.QueryName,
			SpaceViewId = _form.SpaceViewId,
			SpaceViewColumnId = _form.Id,
			EditContext = EditContext,
			ValidationMessageStore = MessageStore
		};
		componentData[TfConstants.SPACE_VIEW_COMPONENT_OPTIONS_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<string>(this, _customOptionsChangedHandler);
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
