namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.FileRepositoryFileUpdateDialog.TfFileRepositoryFileUpdateDialog", "WebVella.Tefter")]
public partial class TfFileRepositoryFileUpdateDialog : TfFormBaseComponent, IDialogContentComponent<TucRepositoryFile>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] private IState<TfAppState> TfAppState { get; set; }
	[Parameter] public TucRepositoryFile Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isSubmitting = false;

	private TucFileForm _form = new();
	private FluentInputFileEventArgs _upload = null;
	private string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());

	FluentInputFile fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_form = new TucFileForm
		{
			Id = Content.Id,
			CreatedBy = TfAppState.Value.CurrentUser.Id,
			LocalFilePath = null,
			FileName = Content.FileName
		};
		base.InitForm(_form);

	}

	private void _onCompleted(IEnumerable<FluentInputFileEventArgs> files)
	{
		Files = files.ToList();
		progressPercent = fileUploader!.ProgressPercent;

		if (Files.Count > 0)
		{
			_upload = Files[0];
		}
	}

	private void _onProgress(FluentInputFileEventArgs e)
	{
		progressPercent = e.ProgressPercent;
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

			////Check form
			_form.LocalFilePath = _upload?.LocalFile.ToString();
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TucRepositoryFile provider;
			var submit = _form with
			{
				Id = _form.Id
			};
			provider = UC.UpdateFile(submit);
			await Dialog.CloseAsync(provider);
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

