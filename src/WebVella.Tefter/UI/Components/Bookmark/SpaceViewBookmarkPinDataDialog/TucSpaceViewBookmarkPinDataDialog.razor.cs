namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewBookmarkPinDataDialog : TfFormBaseComponent, IDialogContentComponent<TfCreatePinDataBookmarkModel?>
{
	[Parameter] public TfCreatePinDataBookmarkModel? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private readonly string _error = string.Empty;
	private bool _isSubmitting = false;
	private TfCreatePinDataBookmarkModel _form = new();
	private List<TfTag> _tags = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Dataset is null) throw new Exception("Dataset is required");
		if (Content.SpacePage is null) throw new Exception("SpacePage is required");
		if (Content.SpaceView is null) throw new Exception("SpaceView is required");
		if (Content.SelectedRowIds.Count == 0) throw new Exception("SelectedRowIds is required");
		_form = Content with { Name = string.Empty };
		_onDescriptionChanged();
		InitForm(_form);
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
			
			var result = TfService.CreateBookmark(_form);
			ToastService.ShowSuccess(LOC($"Records pinned"));
			await TfEventBus.PublishAsync(
				key: TfAuthLayout.GetUserId(),
				payload: new TfBookmarkCreatedEventPayload(result));
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

	private void _onDescriptionChanged()
	{
		_tags.Clear();
		if (!String.IsNullOrWhiteSpace(_form.Description))
		{
			foreach (var item in TfConverters.GetUniqueTagsFromText(_form.Description))
			{
				_tags.Add(new TfTag { Id = Guid.NewGuid(), Label = item });
			}
		}
	}
}