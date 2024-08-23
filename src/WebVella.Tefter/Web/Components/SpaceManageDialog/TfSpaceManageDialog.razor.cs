namespace WebVella.Tefter.Web.Components.SpaceManageDialog;
public partial class TfSpaceManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpace>
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpace Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		base.InitForm(UC.SpaceManageForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create space") : LOC("Manage space");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (_isCreate)
			{
				UC.SpaceManageForm = UC.SpaceManageForm with { Id = Guid.NewGuid() };
			}
			else
			{

				UC.SpaceManageForm = new TucSpace()
				{
					Id = Content.Id,
					Name = Content.Name,
					Position = Content.Position,
					IsPrivate = Content.IsPrivate,
					Color = Content.Color,
					IconString = Content.IconString,
				};
			}
			base.InitForm(UC.SpaceManageForm);
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

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new Result<TucSpace>();
			if (_isCreate)
			{
				result = UC.CreateSpaceWithForm(UC.SpaceManageForm);
			}
			else
			{
				result = UC.UpdateSpaceWithForm(UC.SpaceManageForm);
			}

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				await Dialog.CloseAsync(result.Value);
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
