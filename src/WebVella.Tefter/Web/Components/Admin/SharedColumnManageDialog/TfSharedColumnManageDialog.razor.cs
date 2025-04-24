namespace WebVella.Tefter.Web.Components;
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

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create shared column") : LOC("Manage shared column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);

		if(!_isCreate)
		{
			var dbName = Content.DbName;
			if (dbName.StartsWith("sc_"))
			{
				dbName = dbName.Substring(3);
			}

			_form = _form with
			{
				Id = Content.Id,
				DbName = dbName,
				DbType = Content.DbType,
				IncludeInTableSearch = Content.IncludeInTableSearch,
				JoinKeyDbName = Content.JoinKeyDbName,
			};
		}
		else{ 
			_form.DbType = TfAppState.Value.AdminSharedColumnDataTypes.Single(x=> x.TypeValue == TfDatabaseColumnType.Text);
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
			List<TucSharedColumn> sharedColumns;
			var submit = _form with { DbName = "sc_" + _form.DbName };
			if (_isCreate)
			{
				sharedColumns = UC.CreateSharedColumn(submit);
			}
			else
			{
				sharedColumns = UC.UpdateSharedColumn(submit);
			}
			await Dialog.CloseAsync(sharedColumns);
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

