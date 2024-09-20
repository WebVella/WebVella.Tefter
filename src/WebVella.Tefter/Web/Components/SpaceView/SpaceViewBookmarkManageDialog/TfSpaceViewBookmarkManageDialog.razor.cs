namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.TfSpaceViewBookmarkManageDialog.SpaceViewBookmarkManageDialog", "WebVella.Tefter")]
public partial class TfSpaceViewBookmarkManageDialog : TfFormBaseComponent, IDialogContentComponent<TucBookmark>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucBookmark Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TucBookmark _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceViewId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create Bookmark in {0}", TfAppState.Value.SpaceView.Name) : LOC("Manage Bookmark in {0}", TfAppState.Value.Space.Name);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

		if (_isCreate)
		{
			_form = new TucBookmark()
			{
				Id = Guid.NewGuid(),
				SpaceViewId = Content.SpaceViewId,
			};
		}
		else
		{
			_form = Content with { Id = Content.Id };
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
			//Columns should not be generated on edit
			MessageStore.Clear();

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			Result<TucBookmark> result = null;
			//if (_isCreate)
			//	result = UC.CreateSpaceViewWithForm(_form);
			//else
			//	result = UC.UpdateSpaceViewWithForm(_form);

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
