namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewBookmarkManageDialog.TfSpaceViewBookmarkManageDialog", "WebVella.Tefter")]
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
	private bool _isBookmark = true;
	private TucBookmark _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceViewId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content is null) throw new Exception("Content is null");
		if (!String.IsNullOrWhiteSpace(Content.Url)) _isBookmark = false;
		_title = _isBookmark ? LOC("Bookmark for {0}", TfAppState.Value.SpaceView.Name) : LOC("Saved URL in {0}", TfAppState.Value.Space.Name);
		_btnText = LOC("Save");
		_iconBtn = TfConstants.SaveIcon.WithColor(Color.Neutral);

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
			//Columns should not be generated on edit
			MessageStore.Clear();

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			var result = (new List<TucBookmark>(), new List<TucBookmark>());
			if (_form.Id == Guid.Empty)
			{
				_form.Id = Guid.NewGuid();
				result = await UC.CreateBookmarkAsync(_form);
			}
			else
			{
				result = await UC.UpdateBookmarkAsync(_form);
			}

			var resultObj = new Tuple<TucBookmark, List<TucBookmark>, List<TucBookmark>>(_form, result.Item1, result.Item2);
			await Dialog.CloseAsync(resultObj);
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

	private void _onDescriptionChanged(string description)
	{
		_form.Description = description;
		_form.Tags = new List<TucTag>();
		if (!String.IsNullOrWhiteSpace(description))
		{
			foreach (var item in TfConverters.GetUniqueTagsFromText(description))
			{
				_form.Tags.Add(new TucTag { Id = Guid.NewGuid(), Label = item });
			}
		}
	}

}
