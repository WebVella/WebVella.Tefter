namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewColumnManageDialog.TfSpaceViewColumnManageDialog","WebVella.Tefter")]
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


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
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
				UC.SpaceViewColumnForm = UC.SpaceViewColumnForm with { Id = Guid.NewGuid() };
			}
			else
			{

				UC.SpaceViewColumnForm = Content with { Id = Guid.NewGuid() };
			}
			base.InitForm(UC.SpaceViewColumnForm);
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
			//if (_isCreate)
			//{
			//	result = UC.CreateSpaceWithForm(UC.SpaceManageForm);
			//}
			//else
			//{
			//	result = UC.UpdateSpaceWithForm(UC.SpaceManageForm);
			//}

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

	private async Task _columnTypeChangeHandler(TucSpaceViewColumnType option)
	{
		UC.SpaceViewColumnForm.ColumnType = option;
		if (option is null)
		{
			UC.SpaceViewColumnComponent = null;
		}
		else if (UC.SpaceViewColumnComponent is not null)
		{
			if (!option.SupportedComponentTypes.Contains(UC.SpaceViewColumnComponent))
			{
				UC.SpaceViewColumnComponent = option.DefaultComponentType;
			}
		}
		else
		{
			UC.SpaceViewColumnComponent = option.DefaultComponentType;
		}

		await InvokeAsync(StateHasChanged);
	}
	private async Task _columnComponentChangeHandler(Type option)
	{
		UC.SpaceViewColumnComponent = option;
		await InvokeAsync(StateHasChanged);
	}

	private string _getDataMappingValue(string alias)
	{
		if (UC.SpaceViewColumnForm.DataMapping.ContainsKey(alias))
			return UC.SpaceViewColumnForm.DataMapping[alias];

		return null;
	}

	private Dictionary<string,object> _getColumnComponentContext(){ 
		var componentData = new Dictionary<string,object>();

		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfComponentContext{ 
			Mode = TfComponentMode.Options,
			CustomOptionsJson = UC.SpaceViewColumnForm.CustomOptionsJson,
			DataMapping = UC.SpaceViewColumnForm.DataMapping,
			DataTable = null,
			RowIndex = -1,
			QueryName = UC.SpaceViewColumnForm.QueryName,
			SelectedAddonId = UC.SpaceViewColumnForm.SelectedAddonId,
			SpaceViewId = UC.SpaceViewColumnForm.SpaceViewId
		};
		return componentData;
	}
}
