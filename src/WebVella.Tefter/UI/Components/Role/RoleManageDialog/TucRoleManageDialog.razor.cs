namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.UI.Components.Admin.RoleManageDialog.TfRoleManageDialog", "WebVella.Tefter")]
public partial class TucRoleManageDialog : TfFormBaseComponent, IDialogContentComponent<TfRole?>
{
	[Inject] private ITfUserUIService TfUserUIService { get; set; }
	[Parameter] public TfRole? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;
	private TfRole _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		base.InitForm(_form);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create Role") : LOC("Manage Role");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);

		if (!_isCreate)
		{
			_form = new TfRole()
			{
				Name = Content.Name,
				Id = Content.Id,
				IsSystem = Content.IsSystem
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
			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new TfRole();

			if (_isCreate)
			{
				result = TfUserUIService.CreateRole(_form);
				ToastService.ShowSuccess(LOC("Access role was successfully created!"));
			}
			else
			{
				result = TfUserUIService.UpdateRole(_form);
				ToastService.ShowSuccess(LOC("Access role was successfully created!"));
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
