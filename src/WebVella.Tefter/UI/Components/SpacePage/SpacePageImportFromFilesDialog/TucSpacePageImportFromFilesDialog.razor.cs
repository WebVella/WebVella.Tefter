namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageImportFromFilesDialog : TfBaseComponent,
	IDialogContentComponent<List<FluentInputFileEventArgs>?>
{
	[Parameter] public List<FluentInputFileEventArgs>? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private TfSpacePageCreateFromFileContext _context = null!;
	private TfSpacePageCreateFromFileContextItem _selectedItem = null!;
	private List<TfMenuItem> _menu = new();
	private Dictionary<string, TucPageImportFromFilesDialogStats> _statDict = new();
	private bool _isProcessing = true;
	private bool _showLogMessages = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (TfAuthLayout.GetState().Space is null) throw new Exception("No current Space initialized");
		_context = new(Content);
		if (_context.Items.Count > 0)
			_selectedItem = _context.Items[0];
#pragma warning disable BL0005
		Dialog.Class = "tf-modal-no-body-padding";
#pragma warning restore BL0005
		_initMenu();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _importItem(_selectedItem);
		}
	}

	private async Task _cancel()
	{
		var state = TfAuthLayout.GetState();
		if (state.NavigationState.SpacePageId is null)
		{
			var spaceId = TfAuthLayout.GetState().Space!.Id;
			var spacePages = TfService.GetSpacePages(spaceId);
			if (spacePages.Any())
			{
				Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,
					spaceId, spacePages[0].Id
				));
			}
		}
		await Dialog.CancelAsync();

	}

	private void _initMenu()
	{
		_menu = new();
		_statDict = new();
		foreach (var item in _context.Items)
		{
			_menu.Add(new TfMenuItem()
			{
				Id = $"tf-{Guid.NewGuid()}",
				Text = item.FileName,
				Selected = _selectedItem.LocalPath == item.LocalPath,
				Data = new TfMenuItemData() { ImportFileContext = item },
				OnClick = EventCallback.Factory.Create(this, async () => await _selectItem(item)),
				IconCollapsed = item.GetStatusIcon(),
				IconColor = item.GetStatusColor(),
				SpinIcon = item.Status == TfImportFileToPageContextItemStatus.Processing
			});
			_statDict[item.LocalPath] = new TucPageImportFromFilesDialogStats()
			{
				ProcessedWarning =
					item.ProcessStream.GetProgressLog().Count(x => x.Type == TfProgressStreamItemType.Warning),
				ProcessedError = item.ProcessStream.GetProgressLog()
					.Count(x => x.Type == TfProgressStreamItemType.Error)
			};
		}

		_statDict[Guid.Empty.ToString()] = new TucPageImportFromFilesDialogStats()
		{
			ProcessedSuccess =
				_context.Items.Count(x => x.Status == TfImportFileToPageContextItemStatus.ProcessedSuccess),
			ProcessedError =
				_context.Items.Count(x => x.Status == TfImportFileToPageContextItemStatus.ProcessedWithErrors),
			ProcessedWarning =
				_context.Items.Count(x => x.Status == TfImportFileToPageContextItemStatus.ProcessedWithWarnings)
		};
	}

	private async Task _selectItem(TfSpacePageCreateFromFileContextItem item, bool fromProcess = false)
	{
		if (!fromProcess && _isProcessing) return;
		_selectedItem = item;
		_initMenu();
		await InvokeAsync(StateHasChanged);
	}

	private async Task _importItem(TfSpacePageCreateFromFileContextItem item)
	{
		item.ProcessStream.OnProgress += (message) =>
		{
			InvokeAsync(async () =>
			{
				await _updateProgress(item, message);
			});
		};
		item.User = TfAuthLayout.GetState().User;
		item.Space = TfAuthLayout.GetState().Space!;

		await TfService.SpacePageCreateFromFileAsync(item);

		await InvokeAsync(StateHasChanged);
		await Task.Delay(100);
		var selectedIndex = _context.Items.FindIndex(x => x.LocalPath == _selectedItem.LocalPath);
		if (selectedIndex < _context.Items.Count - 1)
		{
			await _selectItem(_context.Items[selectedIndex + 1], true);
			await _importItem(_selectedItem);
		}
		else
		{
			_initMenu();
			_isProcessing = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _updateProgress(TfSpacePageCreateFromFileContextItem item, TfProgressStreamItem message)
	{
		_initMenu();
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		await InvokeAsync(StateHasChanged);
		try
		{
			await JSRuntime.InvokeVoidAsync("Tefter.scrollToElement", message.Id);
		}
		catch
		{
			// Ignore errors if element is not found or JS is not available
		}
	}


	private record TucPageImportFromFilesDialogStats
	{
		public int ProcessedSuccess { get; set; } = 0;
		public int ProcessedWarning { get; set; } = 0;
		public int ProcessedError { get; set; } = 0;
	}
}