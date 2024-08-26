using WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog;

namespace WebVella.Tefter.Web.Components.SpaceDataManage;
public partial class TfSpaceDataManage : TfFormBaseComponent
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpaceData Form { get; set; }

	internal TucDataProvider _selectedProvider = null;

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
		base.InitForm(Form);
		if (Form is null) throw new Exception("Form is null");
	}

	private void _dataProviderSelected(TucDataProvider provider){ 
		if(provider is null) return;
		_selectedProvider = provider;
		Form.DataProviderId = _selectedProvider.Id;
	}

	private async Task _addFilter()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataFilterManageDialog>(
		new TucFilterBase(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			//var item = (TucSpace)result.Data;
			//ToastService.ShowSuccess(LOC("Space view successfully created!"));
			//Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, item.Id));
		}
	}

}
