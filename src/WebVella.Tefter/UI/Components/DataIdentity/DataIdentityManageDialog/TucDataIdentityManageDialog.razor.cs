namespace WebVella.Tefter.UI.Components;
public partial class TucDataIdentityManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataIdentity?>
{
	[Parameter] public TfDataIdentity? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private bool _isCreate = false;
	private TfDataIdentity _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		base.InitForm(_form);
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataIdentity is null) _isCreate = true;
		_title = _isCreate ? LOC("Create Identity") : LOC("Manage Identity");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;

		if (!_isCreate)
		{
			_form = new TfDataIdentity()
			{
				DataIdentity = Content.DataIdentity ?? String.Empty,
				Label = Content.Label
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

			var result = new TfDataIdentity();

			if (_isCreate)
			{
				result = TfService.CreateDataIdentity(_form);
				ToastService.ShowSuccess(LOC("Data identity created"));
			}
			else
			{
				result = TfService.UpdateDataIdentity(_form);
				ToastService.ShowSuccess(LOC("Data identity updated"));
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
