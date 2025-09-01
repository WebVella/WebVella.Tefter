namespace WebVella.Tefter.Talk.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TalkChannelManageDialog.TalkChannelManageDialog", "WebVella.Tefter.Talk")]
public partial class TalkChannelManageDialog : TfFormBaseComponent, IDialogContentComponent<TalkChannel>
{
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkChannel Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TalkChannel _form = new();
	private List<string> _sharedColumnsOptions = new();
	private List<string> _dataIdentiyOptions = new();
	private Dictionary<string, List<string>> _dataIdentityColumns = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create channel") : LOC("Manage channel");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add") : TfConstants.GetIcon("Save");
		var dataIdentities = TalkService.GetAllDataIdentities();
		var sharedColumns = TalkService.GetAllSharedColumns();

		_dataIdentiyOptions = dataIdentities.Select(x => x.DataIdentity).Order().ToList();

		foreach (var item in dataIdentities)
		{
			_dataIdentityColumns[item.DataIdentity] = sharedColumns.Where(x => x.DataIdentity == item.DataIdentity).Select(x => x.DbName).Order().ToList();
		}
		if (_isCreate)
		{
			_form.Id = Guid.NewGuid();
			if (_dataIdentiyOptions.Count > 0)
			{
				_form.DataIdentity = _dataIdentiyOptions[0];
				_sharedColumnsOptions = _dataIdentityColumns[_form.DataIdentity];
				if (_sharedColumnsOptions.Count > 0)
				{
					_form.CountSharedColumnName = _sharedColumnsOptions[0];
				}
			}
		}
		else
		{

			_form = Content with { Id = Content.Id };
			if (_dataIdentityColumns.ContainsKey(_form.DataIdentity))
				_sharedColumnsOptions = _dataIdentityColumns[_form.DataIdentity];
		}
		base.InitForm(_form);
	}

	private void _dataIdentiyOptionChanged(string dataIdentity)
	{
		_sharedColumnsOptions = _dataIdentityColumns[dataIdentity];
		if (_sharedColumnsOptions.Count > 0)
		{
			_form.CountSharedColumnName = _sharedColumnsOptions[0];
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

			var result = new TalkChannel();
			if (_isCreate)
			{
				result = TalkService.CreateChannel(_form);
			}
			else
			{
				result = TalkService.UpdateChannel(_form);
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

}
