namespace WebVella.Tefter.UI.Components;
public partial class TucAdminFileRepositoryPageContent : TfBaseComponent, IDisposable
{
	[Inject] private ITfFileRepositoryUIService TfFileRepositoryUIService { get; set; } = default!;
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] private ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private List<TfRepositoryFile>? _items = null;

	FluentInputFile fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();
	private TfNavigationState _navState = default!;
	private TfUser? _currentUser = null;
	private string? _search = null;
	private string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	public void Dispose()
	{
		TfFileRepositoryUIService.FileRepositoryCreated -= On_RepositoryFileChanged;
		TfFileRepositoryUIService.FileRepositoryUpdated -= On_RepositoryFileChanged;
		TfFileRepositoryUIService.FileRepositoryDeleted -= On_RepositoryFileChanged;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		await _init();
		TfFileRepositoryUIService.FileRepositoryCreated += On_RepositoryFileChanged;
		TfFileRepositoryUIService.FileRepositoryUpdated += On_RepositoryFileChanged;
		TfFileRepositoryUIService.FileRepositoryDeleted += On_RepositoryFileChanged;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}


	private async void On_RepositoryFileChanged(object? caller, TfRepositoryFile args)
	{
		await _init();
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		_currentUser = await TfUserUIService.GetCurrentUserAsync();
		if (navState is not null)
			_navState = navState;
		else
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		_items = TfFileRepositoryUIService.GetRepositoryFiles(search: _search);

		UriInitialized = _navState.Uri;
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
				var result = TfFileRepositoryUIService.CreateRepositoryFile(new TfFileForm
				{
					Id = null,
					CreatedBy = _currentUser?.Id,
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
			TfFileRepositoryUIService.DeleteRepositoryFile(file.Filename);
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
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, _search}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _copyUri(TfRepositoryFile file)
	{
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", file.Uri);
		ToastService.ShowSuccess(LOC("Tefter Uri copied"));

	}
}