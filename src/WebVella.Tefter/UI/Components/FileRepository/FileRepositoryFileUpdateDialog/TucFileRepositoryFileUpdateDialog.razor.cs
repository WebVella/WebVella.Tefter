﻿namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.FileRepositoryFileUpdateDialog.TfFileRepositoryFileUpdateDialog", "WebVella.Tefter")]
public partial class TucFileRepositoryFileUpdateDialog : TfFormBaseComponent, IDialogContentComponent<TfRepositoryFile?>
{
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] private ITfFileRepositoryUIService TfFileRepositoryUIService { get; set; } = default!;
	[Parameter] public TfRepositoryFile? Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private bool _isSubmitting = false;

	private TfFileForm _form = new();
	private TfUser? _currentUser = default!;
	private FluentInputFileEventArgs? _upload = null;
	private string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());

	FluentInputFile fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_currentUser = await TfUserUIService.GetCurrentUserAsync();
		if(_currentUser == null) throw new Exception("Current user not found");
		_form = new TfFileForm
		{
			Id = Content.Id,
			CreatedBy = _currentUser.Id,
			LocalFilePath = null,
			Filename = Content.Filename
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
			var submit = _form with
			{
				Id = _form.Id
			};
			var item = TfFileRepositoryUIService.UpdateRepositoryFile(submit);
			ToastService.ShowSuccess("File successfully updated");
			await Dialog.CloseAsync(item);
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

