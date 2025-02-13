namespace WebVella.Tefter.Assets.Components;
[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderManageDialog.AssetsFolderManageDialog", "WebVella.Tefter.Assets")]
public partial class AssetsFolderManageDialog : TfFormBaseComponent, IDialogContentComponent<AssetsFolder>
{
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsFolder Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private AssetsFolder _form = new();
	private List<string> _sharedColumnsOptions = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create channel") : LOC("Manage channel");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		if (_isCreate)
		{
			_form = _form with { Id = Guid.NewGuid() };
		}
		else
		{

			_form = Content with { Id = Content.Id };
		}
		base.InitForm(_form);
		if(TfAuxDataState.Value.Data.ContainsKey(AssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY))
			_sharedColumnsOptions = ((List<TfSharedColumn>)TfAuxDataState.Value.Data[AssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY]).Select(x=> x.DbName).ToList();
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

			var result = new AssetsFolder();
			if (_isCreate)
			{
				result = AssetsService.CreateFolder(_form);
			}
			else
			{
				result = AssetsService.UpdateFolder(_form);
			}
			await Dialog.CloseAsync(result);
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

}
