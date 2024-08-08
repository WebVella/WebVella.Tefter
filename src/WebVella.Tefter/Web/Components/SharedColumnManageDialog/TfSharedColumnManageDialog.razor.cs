using WebVella.Tefter.UseCases.SharedColumnsAdmin;

namespace WebVella.Tefter.Web.Components.SharedColumnManageDialog;
public partial class TfSharedColumnManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSharedColumn>
{
	[Inject]
	private SharedColumnsAdminUseCase UC { get; set; }
	[Parameter] public TucSharedColumn Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private DynamicComponent typeSettingsComponent;
	private bool _isCreate = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		base.InitForm(UC.SharedColumnForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create shared column") : LOC("Manage shared column");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? new Icons.Regular.Size20.Add() : new Icons.Regular.Size20.Save();

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (_isCreate)
			{
			}
			else
			{
				var dbName = Content.DbName;
				if(dbName.StartsWith("sk_")){ 
					dbName = dbName.Substring(3);
				}

				UC.SharedColumnForm = UC.SharedColumnForm with
				{
					Id = Content.Id,
					DbName = dbName,
					DbType = Content.DbType,
					AddonId = Content.AddonId,
					IncludeInTableSearch = Content.IncludeInTableSearch,
					SharedKeyDbName = Content.SharedKeyDbName,
				};
			}
			base.InitForm(UC.SharedColumnForm);
			await UC.LoadDataTypeInfoList();
			UC.IsBusy = false;
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

			//Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			Result<TucSharedColumn> submitResult;
			var submit = UC.SharedColumnForm with { DbName = "sk_" + UC.SharedColumnForm.DbName};
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

