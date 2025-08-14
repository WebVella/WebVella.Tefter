namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.SharedColumnManageDialog.TfSharedColumnManageDialog", "WebVella.Tefter")]
public partial class TucSharedColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSharedColumn?>
{
	[Inject] protected ITfSharedColumnUIService TfSharedColumnUIService { get; set; } = default!;
	[Inject] protected ITfDataIdentityUIService TfDataIdentityUIService { get; set; } = default!;
	[Inject] protected ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Parameter] public TfSharedColumn? Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;

	private bool _isCreate = false;

	private TfSharedColumn _form = new();
	private List<string> _allDataIdentities = new();
	private ReadOnlyCollection<DatabaseColumnTypeInfo> _columnTypeOptions = default!;
	private DatabaseColumnTypeInfo _selectedColumnType = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create shared column") : LOC("Manage shared column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		_allDataIdentities = TfDataIdentityUIService.GetDataIdentities().Select(x=> x.DataIdentity).ToList();
		_columnTypeOptions = TfMetaUIService.GetDatabaseColumnTypeInfosList();
		if (!_isCreate)
		{
			var dbName = Content.DbName;
			if (dbName.StartsWith(TfConstants.TF_SHARED_COLUMN_PREFIX))
			{
				dbName = dbName.Substring(3);
			}

			_form = _form with
			{
				Id = Content.Id,
				DbName = dbName,
				DbType = Content.DbType,
				IncludeInTableSearch = Content.IncludeInTableSearch,
				DataIdentity = Content.DataIdentity,
			};
		}
		else
		{
			_form.DbType = TfDatabaseColumnType.Text;
			_form.DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;
		}
		_selectedColumnType = _columnTypeOptions.Single(x=> x.Type == _form.DbType);
		base.InitForm(_form);
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

			//Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TfSharedColumn sharedColumn;
			_form.DbType = _selectedColumnType.Type;
			var submit = _form with { DbName = TfConstants.TF_SHARED_COLUMN_PREFIX + _form.DbName };
			if (_isCreate)
			{
				sharedColumn = TfSharedColumnUIService.CreateSharedColumn(submit);
				ToastService.ShowSuccess(LOC("Shared column successfully created"));
			}
			else
			{
				sharedColumn = TfSharedColumnUIService.UpdateSharedColumn(submit);
				ToastService.ShowSuccess(LOC("Shared column successfully updated"));
			}
			await Dialog.CloseAsync(sharedColumn);
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

