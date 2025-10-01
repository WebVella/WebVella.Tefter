namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataset?>
{
	[Parameter] public TfDataset? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private bool _isCreate = false;

	private TfDataset _form = new();
	private TfDataProvider _provider = null!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProviderId is required");
		if (Content.Id == Guid.Empty) _isCreate = true;

		_provider = TfUIService.GetDataProvider(Content.DataProviderId);
		if(_provider is null) throw new Exception("DataProviderId not found");

		_title = _isCreate ? LOC("Create dataset in {0}", _provider.Name) : LOC("Manage dataset in {0}", _provider.Name);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		if (_isCreate)
			_form = Content with { Id = Guid.NewGuid() };
		else
			_form = Content with { Id = Content.Id };

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

			TfDataset result = null!;
			if (_isCreate)
			{
				result = TfUIService.CreateDataset(_form);
				ToastService.ShowSuccess(LOC("Dataset successfully created!"));
			}
			else
			{
				result = TfUIService.UpdateDataset(_form);
				ToastService.ShowSuccess(LOC("Dataset successfully updated!"));
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
