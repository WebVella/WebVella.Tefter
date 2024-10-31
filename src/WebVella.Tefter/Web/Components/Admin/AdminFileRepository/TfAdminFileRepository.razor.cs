namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminFileRepository.TfAdminFileRepository", "WebVella.Tefter")]
public partial class TfAdminFileRepository : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	FluentInputFile? fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();
	private string _search = null;
	private string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_search = TfAppState.Value.Route.Search;
	}

	private void _onCompleted(IEnumerable<FluentInputFileEventArgs> files)
	{
		Files = files.ToList();
		progressPercent = fileUploader!.ProgressPercent;

		if (Files.Count > 0)
		{
			var file = Files[0];
			try
			{
				var result = UC.CreateFile(new TucFileForm
				{
					Id = null,
					CreatedBy = TfAppState.Value.CurrentUser.Id,
					FilePath = null,
					LocalFilePath = file.LocalFile.ToString(),
					Name = file.Name,
				});
				ProcessServiceResponse(result);
				ToastService.ShowSuccess(LOC("File uploaded successfully!"));
				progressPercent = 0;
				var fileRep = TfAppState.Value.AdminFileRepository;
				fileRep.Add(result.Value);
				fileRep = fileRep.OrderBy(x=> x.FilePath).ToList();
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { AdminFileRepository = fileRep}));
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
			finally
			{
				file.LocalFile?.Delete();
			}
		}
	}

	private void _onProgress(FluentInputFileEventArgs e)
	{
		progressPercent = e.ProgressPercent;
	}
	private async Task _editFile(TucFile file)
	{
		var dialog = await DialogService.ShowDialogAsync<TfFileRepositoryFileUpdateDialog>(
		file,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			ToastService.ShowSuccess(LOC("File successfully updated!"));
		}
	}

	private async Task _deleteFile(TucFile file)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this file deleted?")))
			return;
		try
		{
			Result result = UC.DeleteFile(file);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The file is successfully deleted!"));
				var fileRep = TfAppState.Value.AdminFileRepository.Where(x=> x.Id != file.Id).ToList();
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { AdminFileRepository = fileRep }));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}

	private async Task _searchValueChanged(string search)
	{
		search = search?.Trim();
		if(_search == search) return;
		_search = search;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = _search;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _copyUri(TucFile file){ 
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", file.Uri);
		ToastService.ShowSuccess(LOC("Tefter Uri copied"));
	
	}

}