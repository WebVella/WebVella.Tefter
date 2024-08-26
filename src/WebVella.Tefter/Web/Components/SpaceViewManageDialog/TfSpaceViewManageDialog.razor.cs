namespace WebVella.Tefter.Web.Components.SpaceViewManageDialog;
public partial class TfSpaceViewManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceView>
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpaceView Content { get; set; }
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
		base.InitForm(UC.SpaceViewManageForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create view") : LOC("Manage view");
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
				UC.SpaceViewManageForm = UC.SpaceViewManageForm with { Id = Guid.NewGuid() };
			}
			else
			{

				UC.SpaceViewManageForm = new TucSpaceView()
				{
					Id = Content.Id,
					Name = Content.Name,
				};
			}
			base.InitForm(UC.SpaceViewManageForm);
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
			//if (_isCreate)
			//{
			//	result = UC.CreateSpaceWithForm(UC.SpaceViewManageForm);
			//}
			//else
			//{
			//	result = UC.UpdateSpaceWithForm(UC.SpaceViewManageForm);
			//}

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
