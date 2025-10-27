namespace WebVella.Tefter.UI.Components;

public partial class TucPageImportFromFilesDialog : TfBaseComponent,
	IDialogContentComponent<List<FluentInputFileEventArgs>?>
{
	[Parameter] public List<FluentInputFileEventArgs>? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private TfImportFileToPageContext _context = null!;
	private TfImportFileToPageContextItem _selectedItem = null!;
	private List<TfMenuItem> _menu = new();
	private Dictionary<string, TucPageImportFromFilesDialogStats> _statDict = new();
	private ReadOnlyCollection<ITfDataProviderAddon> _providers = null!;
	private bool _isProcessing = true;
	private bool _showLogMessages = false;


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_providers = TfMetaService.GetDataProviderTypes();
		_context = new(Content);
		if (_context.Items.Count > 0)
			_selectedItem = _context.Items[0];
#pragma warning disable BL0005
		Dialog.Class = "tf-modal-no-body-padding";
#pragma warning restore BL0005
		_initMenu();
		await _importItem(_selectedItem);
	}

	private async Task _cancel()
	{
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
				SpinIcon = item.IsProcessed
			});
			_statDict[item.LocalPath] = new TucPageImportFromFilesDialogStats()
			{
				ProcessedWarning = item.ProcessLog.Count(x => x.Type == TfProgressStreamItemType.Warning),
				ProcessedError = item.ProcessLog.Count(x => x.Type == TfProgressStreamItemType.Error)
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

	private async Task _selectItem(TfImportFileToPageContextItem item, bool fromProcess = false)
	{
		if(!fromProcess && _isProcessing) return;
		_selectedItem = item;
		_initMenu();
		await InvokeAsync(StateHasChanged);
	}

	private async Task _importItem(TfImportFileToPageContextItem item)
	{
		item.ProcessStream.OnProgress += (message) =>
		{
			InvokeAsync(async () =>
			{
				await _updateProgress(item, message);
			});
		};
		item.ProcessLog.Add(new TfProgressStreamItem
		{
			Message = LOC("Checking for suitable Data Processor type..."),
			Type = TfProgressStreamItemType.Normal
		});
		foreach (var provider in _providers)
		{
			if (!await provider.CanBeCreatedFromFile(item))
				continue;

			item.IsProcessed = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			item.DataProvider = provider;
			item.ProcessLog.Add(new TfProgressStreamItem
			{
				Message = LOC("Suitable Data Provider type found: {0}",provider.AddonName),
				Type = TfProgressStreamItemType.Normal
			});			
			await provider.CreatedFromFile(item);
		}

		if (item.DataProvider is null)
		{
			item.ProcessLog.Add(new TfProgressStreamItem
			{
				Message = LOC("No suitable Data Provider that can process the file was found"),
				Type = TfProgressStreamItemType.Error
			});
		}

		item.IsProcessed = false;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1000);
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

	private async Task _updateProgress(TfImportFileToPageContextItem item, TfProgressStreamItem message)
	{
		item.ProcessLog.Add(message);
		_initMenu();
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
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