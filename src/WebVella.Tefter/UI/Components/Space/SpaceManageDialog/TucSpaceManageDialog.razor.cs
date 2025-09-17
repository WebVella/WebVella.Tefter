namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceManageDialog.TfSpaceManageDialog", "WebVella.Tefter")]
public partial class TucSpaceManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpace?>
{
	[Inject] private ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Parameter] public TfSpace? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TfSpace _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create space") : LOC("Manage space");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;
		if (_isCreate)
		{
			_form = _form with { Id = Guid.NewGuid(), Color =  TfConstants.DefaultThemeColor, IsPrivate = false };
		}
		else
		{

			_form = new TfSpace()
			{
				Id = Content.Id,
				Name = Content.Name,
				Position = Content.Position,
				IsPrivate = Content.IsPrivate,
				Color = Content.Color,
				FluentIconName = Content.FluentIconName,
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

			var result = new TfSpace();
			if (_isCreate)
			{
				result = TfSpaceUIService.CreateSpace(_form);
				ToastService.ShowSuccess(LOC("Space created successfully!"));
			}
			else
			{
				result = TfSpaceUIService.UpdateSpace(_form);
				ToastService.ShowSuccess(LOC("Space updated successfully!"));
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
