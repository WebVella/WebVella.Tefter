namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewColumnManageDialog.TfSpaceViewColumnManageDialog", "WebVella.Tefter")]
public partial class TfSpaceViewColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceViewColumn>
{
	[Inject] private SpaceUseCase UC { get; set; }
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


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		UC.IsBusy = true;
		base.InitForm(UC.SpaceViewColumnForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create column") : LOC("Manage column");
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
				TucSpaceViewColumnType defaultColumnType = null;
				if (UC.AvailableColumnTypes is not null && UC.AvailableColumnTypes.Any())
				{
					defaultColumnType = UC.AvailableColumnTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
					if (defaultColumnType is null) defaultColumnType = UC.AvailableColumnTypes[0];
				}
				UC.SpaceViewColumnForm = UC.SpaceViewColumnForm with
				{
					Id = Guid.NewGuid(),
					SpaceViewId = Content.SpaceViewId,
					ColumnType = defaultColumnType,
					ComponentType = defaultColumnType?.DefaultComponentType
				};
			}
			else
			{

				UC.SpaceViewColumnForm = Content with { Id = Content.Id };
			}
			base.InitForm(UC.SpaceViewColumnForm);
			_renderComponentTypeSelect = true;
			UC.IsBusy = false;
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

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new Result<TucSpaceViewColumn>();
			if (_isCreate)
			{
				result = UC.CreateSpaceViewColumnWithForm(UC.SpaceViewColumnForm);
			}
			else
			{
				result = UC.UpdateSpaceViewColumnWithForm(UC.SpaceViewColumnForm);
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

	private async Task _columnTypeChangeHandler(string columnTypeId)
	{
		_renderComponentTypeSelect = false;
		UC.SpaceViewColumnForm.ColumnType = null;
		if (!String.IsNullOrWhiteSpace(columnTypeId))
			UC.SpaceViewColumnForm.ColumnType = UC.AvailableColumnTypes.FirstOrDefault(x => x.Id.ToString() == columnTypeId);

		if (UC.SpaceViewColumnForm.ColumnType is null)
			UC.SpaceViewColumnForm.ColumnType = UC.AvailableColumnTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);


		if (UC.SpaceViewColumnForm.ColumnType is null)
		{
			UC.SpaceViewColumnForm.ComponentType = null;
		}
		//else if (UC.SpaceViewColumnForm.ComponentType is not null)
		//{
		//	//select the current value from the new list of objects
		//	Type selectedType = null;
		//	foreach (var supType in option.SupportedComponentTypes)
		//	{
		//		if (supType.FullName == UC.SpaceViewColumnForm.ComponentType.FullName)
		//		{
		//			selectedType = supType;
		//			break;
		//		}
		//	}
		//	if (selectedType is null)
		//	{
		//		UC.SpaceViewColumnForm.ComponentType = option.DefaultComponentType;
		//	}
		//	else
		//	{
		//		UC.SpaceViewColumnForm.ComponentType = selectedType;
		//	}
		//}
		else
		{
			UC.SpaceViewColumnForm.ComponentType = UC.SpaceViewColumnForm.ColumnType.DefaultComponentType;
		}
		_renderComponentTypeSelect = true;
		await InvokeAsync(StateHasChanged);
	}
	private void _columnComponentChangeHandler(string componentTypeFullName)
	{
		UC.SpaceViewColumnForm.ComponentType = null;
		if (UC.SpaceViewColumnForm.ColumnType is not null)
		{
			if (!String.IsNullOrWhiteSpace(componentTypeFullName))
				UC.SpaceViewColumnForm.ComponentType = UC.SpaceViewColumnForm.ColumnType.SupportedComponentTypes.FirstOrDefault(x => x.FullName == componentTypeFullName);

			if (UC.SpaceViewColumnForm.ComponentType is null)
				UC.SpaceViewColumnForm.ComponentType = UC.SpaceViewColumnForm.ColumnType.DefaultComponentType;
		}
	}

	private string _getDataMappingValue(string alias)
	{
		if (UC.SpaceViewColumnForm.DataMapping.ContainsKey(alias))
			return UC.SpaceViewColumnForm.DataMapping[alias];

		return null;
	}

	private Dictionary<string, object> _getColumnComponentContext()
	{
		var componentData = new Dictionary<string, object>();

		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfComponentContext
		{
			Mode = TfComponentMode.Options,
			CustomOptionsJson = UC.SpaceViewColumnForm.CustomOptionsJson,
			DataMapping = UC.SpaceViewColumnForm.DataMapping,
			DataRow = null,
			RowIndex = -1,
			QueryName = UC.SpaceViewColumnForm.QueryName,
			SelectedAddonId = UC.SpaceViewColumnForm.SelectedAddonId,
			SpaceViewId = UC.SpaceViewColumnForm.SpaceViewId,
			EditContext = EditContext,
			ValidationMessageStore = MessageStore
		};
		componentData[TfConstants.SPACE_VIEW_COMPONENT_VALUE_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<string>(this, _customOptionsChangedHandler);
		return componentData;
	}

	private async Task _customOptionsChangedHandler(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) UC.SpaceViewColumnForm.CustomOptionsJson = null;

		if (!(value.StartsWith("{") && value.StartsWith("{"))
		|| (value.StartsWith("[") && value.StartsWith("]")))
		{
			ToastService.ShowError("custom options value needs to be json");
			return;
		}

		UC.SpaceViewColumnForm.CustomOptionsJson = value;
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

}
