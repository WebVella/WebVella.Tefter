using Microsoft.AspNetCore.Identity;

namespace WebVella.Tefter.UI.Components;
public partial class TucSharedColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSharedColumn?>
{
	[Parameter] public TfSharedColumn? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;

	private bool _isCreate = false;

	private TfSharedColumn _form = new();
	private List<string> _allDataIdentities = new();
	private ReadOnlyCollection<DatabaseColumnTypeInfo> _columnTypeOptions = null!;
	private DatabaseColumnTypeInfo _selectedColumnType = null!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create shared column") : LOC("Manage shared column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		_allDataIdentities = TfUIService.GetDataIdentities().Select(x=> x.DataIdentity).ToList();
		_columnTypeOptions = WebVella.Tefter.Services.TfService.GetDatabaseColumnTypeInfosList();
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
			_form.DataIdentity = _allDataIdentities.Count > 0 ? _allDataIdentities[0] : String.Empty;
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
				sharedColumn = TfService.CreateSharedColumn(submit);
				ToastService.ShowSuccess(LOC("Shared column successfully created"));
			}
			else
			{
				sharedColumn = TfService.UpdateSharedColumn(submit);
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


	private async Task addDataIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataIdentityManageDialog>(
		new TfDataIdentity(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TfDataIdentity)result.Data;
			_allDataIdentities.Add(item.DataIdentity);
			await InvokeAsync(StateHasChanged);
			await Task.Delay(10);
			_form.DataIdentity = item.DataIdentity;
			await InvokeAsync(StateHasChanged);
		}

	}
}

