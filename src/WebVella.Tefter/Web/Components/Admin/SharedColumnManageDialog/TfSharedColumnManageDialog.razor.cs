namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SharedColumnManageDialog.TfSharedColumnManageDialog", "WebVella.Tefter")]
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

	private DynamicComponent typeSettingsComponent;
	private bool _isCreate = false;

	private TucSharedColumnForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create shared column") : LOC("Manage shared column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

		if(!_isCreate)
		{
			var dbName = Content.DbName;
			if (dbName.StartsWith("sk_"))
			{
				dbName = dbName.Substring(3);
			}

			_form = _form with
			{
				Id = Content.Id,
				DbName = dbName,
				DbType = Content.DbType,
				AddonId = Content.AddonId,
				IncludeInTableSearch = Content.IncludeInTableSearch,
				SharedKeyDbName = Content.SharedKeyDbName,
			};
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
			Result<List<TucSharedColumn>> submitResult;
			var submit = _form with { DbName = "sk_" + _form.DbName };
			if (_isCreate)
			{
				submitResult = UC.CreateSharedColumn(submit);
			}
			else
			{
				submitResult = UC.UpdateSharedColumn(submit);
			}

			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				await Dialog.CloseAsync(submitResult.Value);
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

}

