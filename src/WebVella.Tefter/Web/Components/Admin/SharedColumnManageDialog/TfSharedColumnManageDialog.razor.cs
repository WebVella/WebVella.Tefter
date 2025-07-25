﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.SharedColumnManageDialog.TfSharedColumnManageDialog", "WebVella.Tefter")]
public partial class TfSharedColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSharedColumn>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSharedColumn Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private bool _isCreate = false;

	private TucSharedColumnForm _form = new();
	private List<string> _allDataIdentities = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create shared column") : LOC("Manage shared column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		_allDataIdentities = (await UC.GetDataIdentitiesAsync()).Select(x=> x.Name).ToList();
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
			_form.DbType = TfAppState.Value.AdminSharedColumnDataTypes.Single(x => x.TypeValue == TfDatabaseColumnType.Text);
			_form.DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;
		}
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
			TucSharedColumn sharedColumn;
			var submit = _form with { DbName = TfConstants.TF_SHARED_COLUMN_PREFIX + _form.DbName };
			if (_isCreate)
			{
				sharedColumn = UC.CreateSharedColumn(submit);
			}
			else
			{
				sharedColumn = UC.UpdateSharedColumn(submit);
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

