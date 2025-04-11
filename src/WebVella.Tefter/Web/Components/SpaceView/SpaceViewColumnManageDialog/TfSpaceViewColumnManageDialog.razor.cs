namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewColumnManageDialog.TfSpaceViewColumnManageDialog", "WebVella.Tefter")]
public partial class TfSpaceViewColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceViewColumn>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSpaceViewColumn Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	//NOTE: this changes the Items of the component type select
	//there is a bug and the component needs to be rerendered when both value and items ara changed
	private bool _renderComponentTypeSelect = false;
	private string _activeTab = "data";
	private TucSpaceViewColumn _form = new();
	private List<string> _options = new();
	private TucSpaceViewColumnType _selectedColumnType = null;
	private List<TucSpaceViewColumnComponent> _selectedColumnTypeComponents = null;
	private TucSpaceViewColumnComponent _selectedColumnComponent = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		if (_isCreate)
		{
			_form = _form with
			{
				Id = Guid.NewGuid(),
				QueryName = NavigatorExt.GenerateQueryName(),
				SpaceViewId = Content.SpaceViewId,
				TypeId = new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_TYPE_ID),
				ComponentId = new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_COMPONENT_ID),
			};
		}
		else
		{

			_form = Content with { Id = Content.Id };
		}
		if (_form.ComponentId == Guid.Empty)
			_form.ComponentId = new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_COMPONENT_ID);

		_selectComponentType(_form.TypeId);

		base.InitForm(_form);
		_renderComponentTypeSelect = true;
		TucSpaceData selectedSpaceData = null;
		if (TfAppState.Value.SpaceDataList is not null)
		{
			selectedSpaceData = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId);
		}
		if (selectedSpaceData is not null)
		{
			if (selectedSpaceData.Columns.Count > 0)
				_options = selectedSpaceData.Columns;
			else
			{
				//This space data uses all the columns from the data provider
				var dataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == selectedSpaceData.DataProviderId);
				if (dataProvider is not null)
				{
					_options.AddRange(dataProvider.ColumnsPublic.Select(x => x.DbName));
				}
			}
		}
	}

	private void _selectComponentType(Guid typeId)
	{
		_selectedColumnType = UC.GetSpaceViewColumnTypeById(typeId);
		if (_selectedColumnType is not null)
			_selectedColumnTypeComponents = UC.GetSpaceViewColumnTypeSupportedComponents(_selectedColumnType.Id);
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

			var result = new List<TucSpaceViewColumn>();
			if (_isCreate)
			{
				result = UC.CreateSpaceViewColumnWithForm(_form);
			}
			else
			{
				result = UC.UpdateSpaceViewColumnWithForm(_form);
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

	private async Task _columnTypeChangeHandler(TucSpaceViewColumnType columnType)
	{
		_renderComponentTypeSelect = false;
		_selectedColumnType = null;
		_form.TypeId = Guid.Empty;
		if (columnType is not null)
		{
			_selectedColumnType = columnType;
			_form.TypeId = columnType.Id;
		}

		if (_selectedColumnType is null)
		{
			_selectedColumnType = UC.GetSpaceViewColumnTypeById(new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
			_form.TypeId = _selectedColumnType.Id;
		}
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		_renderComponentTypeSelect = true;
		await InvokeAsync(StateHasChanged);
	}
	private void _columnComponentChangeHandler(TucSpaceViewColumnComponent componentType)
	{
		_selectedColumnComponent = null;
		_form.ComponentId = Guid.Empty;
		if (_selectedColumnType is not null)
		{
			if (componentType is not null)
			{
				_selectedColumnComponent = componentType;
				_form.ComponentId = componentType.Id;
			}

			if (_selectedColumnComponent is null)
			{
				Guid defaultCompId = _selectedColumnType.DefaultComponentId is not null ? _selectedColumnType.DefaultComponentId.Value
					: new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_COMPONENT_ID);
				_selectedColumnComponent = UC.GetSpaceViewColumnComponentById(defaultCompId);
				_form.ComponentId = _selectedColumnComponent.Id;
			}
		}
	}

	private string _getDataMappingValue(string alias)
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

		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegion
		{
			Hash = TfAppState.Value.Hash,
			Mode = TucComponentMode.Options,
			CustomOptionsJson = _form.CustomOptionsJson,
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
		if (String.IsNullOrWhiteSpace(value)) _form.CustomOptionsJson = null;

		if (!(value.StartsWith("{") && value.StartsWith("{"))
		|| (value.StartsWith("[") && value.StartsWith("]")))
		{
			ToastService.ShowError("custom options value needs to be json");
			return;
		}

		_form.CustomOptionsJson = value;
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
