using WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog;

namespace WebVella.Tefter.Web.Components.SpaceDataManage;
public partial class TfSpaceDataManage : TfFormBaseComponent
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpaceData Form { get; set; }

	internal TucDataProvider _selectedProvider = null;

	internal List<string> _allColumnOptions = new List<string> { "Boz", "Boz2", "Boz3" };
	internal List<string> _columnOptions
	{
		get
		{
			if (Form is null || Form.Columns is null) return _allColumnOptions;
			return _allColumnOptions.Where(x => !Form.Columns.Contains(x)).ToList();
		}
	}
	internal string _selectedColumn = null;

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

	private void _dataProviderSelected(TucDataProvider provider)
	{
		if (provider is null) return;
		_selectedProvider = provider;
		Form.DataProviderId = _selectedProvider.Id;
	}


	private void _addColumn()
	{
		try
		{
			if (String.IsNullOrWhiteSpace(_selectedColumn)) return;
			if (Form.Columns.Contains(_selectedColumn)) return;
			Form.Columns.Add(_selectedColumn);
			Form.Columns = Form.Columns.Order().ToList();
		}
		finally
		{
			_selectedColumn = null;
		}
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

	private void _deleteColumn(string column)
	{
		if (String.IsNullOrWhiteSpace(column)) return;
		if (!Form.Columns.Contains(column)) return;
		Form.Columns.Remove(column);
	}
}
