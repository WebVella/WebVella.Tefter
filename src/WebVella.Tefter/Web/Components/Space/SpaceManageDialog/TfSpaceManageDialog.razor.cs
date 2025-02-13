namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceManageDialog.TfSpaceManageDialog", "WebVella.Tefter")]
public partial class TfSpaceManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpace>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSpace Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TucSpace _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create space") : LOC("Manage space");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		if (_isCreate)
		{
			_form = _form with { Id = Guid.NewGuid() };
		}
		else
		{

			_form = new TucSpace()
			{
				Id = Content.Id,
				Name = Content.Name,
				Position = Content.Position,
				IsPrivate = Content.IsPrivate,
				Color = Content.Color,
				IconString = Content.IconString,
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

			var result = new TucSpace();
			if (_isCreate)
			{
				result = UC.CreateSpaceWithForm(_form);
			}
			else
			{
				result = UC.UpdateSpaceWithForm(_form);
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
