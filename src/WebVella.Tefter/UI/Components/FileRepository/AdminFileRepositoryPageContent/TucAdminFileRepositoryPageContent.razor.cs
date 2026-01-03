namespace WebVella.Tefter.UI.Components;

public partial class TucAdminFileRepositoryPageContent : TfBaseComponent, IAsyncDisposable
{
	private List<TfRepositoryFile>? _items = null;

	private FluentInputFile _fileUploader = null!;
	private int _progressPercent = 0;
	private List<FluentInputFileEventArgs> _files = new();
	private TfNavigationState _navState = null!;
	private string? _search = null;
	private readonly string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private FluentSearch? _refSearch = null;
	private IAsyncDisposable _repositoryFileEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _repositoryFileEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_repositoryFileEventSubscriber = await TfEventBus.SubscribeAsync<TfRepositoryFileEventPayload>(
			handler: On_RepositoryFileEventAsync);
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

	private async Task On_RepositoryFileEventAsync(string? key, TfRepositoryFileEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
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
		_files = files.ToList();
		_progressPercent = _fileUploader.ProgressPercent;

		if (_files.Count > 0)
		{
			var file = _files[0];
			try
			{
				_ = TfService.CreateRepositoryFile(new TfFileForm
				{
					Id = null,
					CreatedBy = TfAuthLayout.GetState().User.Id,
					LocalFilePath = file.LocalFile?.ToString(),
					Filename = file.Name,
				});

				ToastService.ShowSuccess(LOC("File uploaded successfully!"));

				_progressPercent = 0;
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
			finally
			{
				_progressPercent = 0;
				file.LocalFile?.Delete();
			}
		}
	}

	private void _onProgress(FluentInputFileEventArgs e)
	{
		_progressPercent = e.ProgressPercent;
	}

	private async Task _editFile(TfRepositoryFile file)
	{
		var dialog = await DialogService.ShowDialogAsync<TucFileRepositoryFileUpdateDialog>(
			file,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
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
			ToastService.ShowSuccess(LOC("The file was successfully deleted!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _searchValueChanged(string? search)
	{
		search = search?.Trim();
		if (_search == search) return;
		_search = search;
		var queryDict = new Dictionary<string, object?> { { TfConstants.SearchQueryName, _search } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _copyUri(TfRepositoryFile file)
	{
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", file.Uri);
		ToastService.ShowSuccess(LOC("Tefter Uri copied"));
	}
}