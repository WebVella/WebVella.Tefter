namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageRenameDialog : TfBaseComponent, IDialogContentComponent<TfSpacePage?>
{
	[Parameter] public TfSpacePage? Content { get; set; } = null;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _name = String.Empty;
	private List<ValidationError> _validationErrors { get; set; } = new();

	private FluentTextField _ref  = null!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Task.Delay(100);
			_ref.FocusAsync();
		}
	}

	private void _init()
	{
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) throw new Exception("Page Id is required");
		_name = Content.Name;
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;

		//Get dynamic settings component errors
		_validationErrors = new();
		if (String.IsNullOrWhiteSpace(_name))
			_validationErrors.Add(new ValidationError(nameof(_name), "required"));

		//Check form
		if (_validationErrors.Count > 0) return;


		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);

		try
		{
			TfService.RenameSpacePage(
				pageId: Content!.Id,
				name: _name
			);

			await Dialog.CloseAsync();
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
}