namespace WebVella.Tefter.UI.Components;

public partial class TucFileRepositoryFileSelectDialog : TfBaseComponent, IDialogContentComponent<string?>
{
	[Parameter] public string? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private bool _isSubmitting = false;

	private FluentInputFileEventArgs? _upload = null;
	private string _uploadId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	FluentInputFile fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();
	private string? _search = null;
	private FluentSearch? _refSearch = null;
	private bool _isLoading = true;
	private List<TfRepositoryFile> _items = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender && _refSearch != null)
		{
			_refSearch.FocusAsync();
		}
	}

	private void _init()
	{
		try
		{
			_items = TfUIService.GetRepositoryFiles(search: _search);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isLoading = false;
			StateHasChanged();
		}
	}

	private async Task _onCompleted(IEnumerable<FluentInputFileEventArgs> files)
	{
		Files = files.ToList();
		progressPercent = fileUploader!.ProgressPercent;

		if (Files.Count > 0)
		{
			var file = Files[0];
			try
			{
				var result = TfUIService.CreateRepositoryFile(new TfFileForm
				{
					Id = null,
					CreatedBy = TfAuthLayout.CurrentUser?.Id,
					LocalFilePath = file.LocalFile.ToString(),
					Filename = file.Name,
				});
				await Dialog.CloseAsync(result.Uri.ToString());
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

	private async Task _selectFile(TfRepositoryFile file)
	{
		await Dialog.CloseAsync(file.Uri.ToString());
	}


	private void _searchValueChanged(string? search)
	{
		search = search is not null ? search?.Trim() : null;
		if (_search == search) return;
		_search = search;
		_init();
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

