﻿namespace WebVella.Tefter.Web.Components;
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
			TucSpaceViewColumnType defaultColumnType = null;
			if (TfAppState.Value.AvailableColumnTypes is not null && TfAppState.Value.AvailableColumnTypes.Any())
			{
				defaultColumnType = TfAppState.Value.AvailableColumnTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
				if (defaultColumnType is null) defaultColumnType = TfAppState.Value.AvailableColumnTypes[0];
			}
			_form = _form with
			{
				Id = Guid.NewGuid(),
				QueryName = NavigatorExt.GenerateQueryName(),
				SpaceViewId = Content.SpaceViewId,
				ColumnType = defaultColumnType with { Id = defaultColumnType.Id },
				ComponentType = defaultColumnType?.DefaultComponentType
			};
		}
		else
		{

			_form = Content with { Id = Content.Id };
		}
		if (_form.ComponentType is null && _form.ColumnType is not null)
			_form.ComponentType = _form.ColumnType.DefaultComponentType;
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

			var result = new Result<List<TucSpaceViewColumn>>();
			if (_isCreate)
			{
				result = UC.CreateSpaceViewColumnWithForm(_form);
			}
			else
			{
				result = UC.UpdateSpaceViewColumnWithForm(_form);
			}

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				await Dialog.CloseAsync(result.Value);
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

	private async Task _columnTypeChangeHandler(TucSpaceViewColumnType columnType)
	{
		_renderComponentTypeSelect = false;
		_form.ColumnType = null;
		if (columnType is not null)
			_form.ColumnType = columnType;

		if (_form.ColumnType is null)
			_form.ColumnType = TfAppState.Value.AvailableColumnTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);


		if (_form.ColumnType is null)
		{
			_form.ComponentType = null;
		}
		//else if (_form.ComponentType is not null)
		//{
		//	//select the current value from the new list of objects
		//	Type selectedType = null;
		//	foreach (var supType in option.SupportedComponentTypes)
		//	{
		//		if (supType.FullName == _form.ComponentType.FullName)
		//		{
		//			selectedType = supType;
		//			break;
		//		}
		//	}
		//	if (selectedType is null)
		//	{
		//		_form.ComponentType = option.DefaultComponentType;
		//	}
		//	else
		//	{
		//		_form.ComponentType = selectedType;
		//	}
		//}
		else
		{
			_form.ComponentType = _form.ColumnType?.DefaultComponentType;
		}
		_renderComponentTypeSelect = true;
		await InvokeAsync(StateHasChanged);
	}
	private void _columnComponentChangeHandler(Type componentType)
	{
		_form.ComponentType = null;
		if (_form.ColumnType is not null)
		{
			if (componentType is not null)
				_form.ComponentType = componentType;

			if (_form.ComponentType is null)
				_form.ComponentType = _form.ColumnType?.DefaultComponentType;
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

		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TucViewColumnComponentContext
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
