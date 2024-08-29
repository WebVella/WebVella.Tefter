namespace WebVella.Tefter.Web.Components.SpaceDataManageDialog;
public partial class TfSpaceDataManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceData>
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpaceData Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	private TucDataProvider _selectedDataProvider = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		await UC.Init(this.GetType(), Content.SpaceId);
		base.InitForm(UC.SpaceDataManageForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create dataset in {0}", UC.SpaceName) : LOC("Manage dataset in {0}", UC.SpaceName);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			//space id is in conntent
			if (_isCreate)
				UC.SpaceDataManageForm = Content with { Id = Guid.NewGuid() };
			else
				UC.SpaceDataManageForm = Content with { Id = Content.Id };

			if (UC.SpaceDataManageForm.DataProviderId != Guid.Empty)
				_selectedDataProvider = UC.AllDataProviders.FirstOrDefault(x => x.Id == UC.SpaceDataManageForm.DataProviderId);

			base.InitForm(UC.SpaceDataManageForm);
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

			Result<TucSpaceData> result = null;
			if (_isCreate)
				result = UC.CreateSpaceDataWithForm(UC.SpaceDataManageForm);
			else
				result = UC.UpdateSpaceDataWithForm(UC.SpaceDataManageForm);

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

	private void _dataProviderSelected(TucDataProvider provider)
	{
		if (provider == null) return;
		_selectedDataProvider = provider;
		UC.SpaceDataManageForm.DataProviderId = provider.Id;
	}
}
