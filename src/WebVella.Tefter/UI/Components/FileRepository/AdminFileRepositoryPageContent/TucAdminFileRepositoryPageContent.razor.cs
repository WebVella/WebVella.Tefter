namespace WebVella.Tefter.UI.Components;

public partial class TucAdminFileRepositoryPageContent : TfBaseComponent, IDisposable
{
	private List<TfRepositoryFile>? _items = null;

	FluentInputFile fileUploader = null!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();
	private TfNavigationState _navState = null!;
	private string? _search = null;
	private string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());

	private FluentSearch? _refSearch = null;

	public void Dispose()
	{
		TfEventProvider.RepositoryFileCreatedEvent -= On_RepositoryFileChanged;
		TfEventProvider.RepositoryFileUpdatedEvent -= On_RepositoryFileChanged;
		TfEventProvider.RepositoryFileDeletedEvent -= On_RepositoryFileChanged;
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.RepositoryFileCreatedEvent += On_RepositoryFileChanged;
		TfEventProvider.RepositoryFileUpdatedEvent += On_RepositoryFileChanged;
		TfEventProvider.RepositoryFileDeletedEvent += On_RepositoryFileChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender && _refSearch != null)
		{
			_refSearch.FocusAsync();
		}
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}


	private async Task On_RepositoryFileChanged(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_navState = navState;
			_items = TfService.GetRepositoryFiles(filenameContains: _search);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
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
				var result = TfService.CreateRepositoryFile(new TfFileForm
				{
					Id = null,
					CreatedBy = TfAuthLayout.GetState().User?.Id,
					LocalFilePath = file.LocalFile.ToString(),
					Filename = file.Name,
				});

				ToastService.ShowSuccess(LOC("File uploaded successfully!"));

				progressPercent = 0;
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
			finally
			{
				progressPercent = 0;
				file.LocalFile?.Delete();
			}
		}
	}

	private void _onProgress(FluentInputFileEventArgs e)
	{
		progressPercent = e.ProgressPercent;
	}

	private async Task _editFile(TfRepositoryFile file)
	{
		var dialog = await DialogService.ShowDialogAsync<TucFileRepositoryFileUpdateDialog>(
			file,
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			ToastService.ShowSuccess(LOC("File successfully updated!"));
		}
	}

	private async Task _deleteFile(TfRepositoryFile file)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this file deleted?")))
			return;
		try
		{
			TfService.DeleteRepositoryFile(file.Filename);
			ToastService.ShowSuccess(LOC("The file is successfully deleted!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _searchValueChanged(string search)
	{
		search = search?.Trim();
		if (_search == search) return;
		_search = search;
		var queryDict = new Dictionary<string, object> { { TfConstants.SearchQueryName, _search } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _copyUri(TfRepositoryFile file)
	{
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", file.Uri);
		ToastService.ShowSuccess(LOC("Tefter Uri copied"));
	}
}