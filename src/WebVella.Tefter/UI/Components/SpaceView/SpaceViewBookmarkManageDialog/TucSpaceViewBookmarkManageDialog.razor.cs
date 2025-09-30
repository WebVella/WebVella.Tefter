namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewBookmarkManageDialog : TfFormBaseComponent, IDialogContentComponent<TfBookmark?>
{
	[Parameter] public TfBookmark? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isBookmark = true;
	private TfBookmark _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceViewId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content is null) throw new Exception("Content is null");
		if (!String.IsNullOrWhiteSpace(Content.Url)) _isBookmark = false;
		_title = _isBookmark ? LOC("Bookmark") : LOC("Saved URL");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save")!;

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
			var result = (new List<TfBookmark>(), new List<TfBookmark>());
			if (_form.Id == Guid.Empty)
			{
				_form.Id = Guid.NewGuid();
				TfUIService.CreateBookmark(_form);
				ToastService.ShowSuccess(LOC($"{(_isBookmark ? "Bookmark" : "URL")} saved"));
			}
			else
			{
				TfUIService.UpdateBookmark(_form);
				ToastService.ShowSuccess(LOC($"{(_isBookmark ? "Bookmark" : "URL")} updated"));
			}

			await Dialog.CloseAsync(_form);
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
		_form.Tags = new List<TfTag>();
		if (!String.IsNullOrWhiteSpace(description))
		{
			foreach (var item in TfConverters.GetUniqueTagsFromText(description))
			{
				_form.Tags.Add(new TfTag { Id = Guid.NewGuid(), Label = item });
			}
		}
	}

}
