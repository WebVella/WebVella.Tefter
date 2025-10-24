namespace WebVella.Tefter.UI.Components;

public partial class TucPageImportFromFilesDialog : TfBaseComponent,
	IDialogContentComponent<List<FluentInputFileEventArgs>?>
{
	[Parameter] public List<FluentInputFileEventArgs>? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private TfImportFileToPageContext _context = null!;
	private TfImportFileToPageContextItem _selectedItem = null!;
	private List<TfMenuItem> _menu = new();
	private List<TfMenuItem> _steps = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");

		_context = new(Content);

		if (_context.Items.Count > 0)
			_selectedItem = _context.Items[0];
#pragma warning disable BL0005
		Dialog.Class = "tf-modal-no-body-padding";
#pragma warning restore BL0005
		_initMenu();
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private void _initMenu()
	{
		_menu = new();
		foreach (var item in _context.Items)
		{
			_menu.Add(new TfMenuItem()
			{
				Id = $"tf-{Guid.NewGuid()}",
				Text = item.FileName,
				Selected = _selectedItem.LocalPath == item.LocalPath,
				Data = new TfMenuItemData() { ImportFileContext = item },
				OnClick = EventCallback.Factory.Create(this, async () => await _menuItemClickHandler(item)),
				IconCollapsed = item.GetStatusIcon()
			});
		}

		_steps = new();
		_steps.Add(new TfMenuItem()
		{
			Id= $"tf-{Guid.NewGuid()}",
			Text = LOC("Data Provider"),
			Description = LOC("data processing method"),
			Selected = TfImportFileToPageContextItemStep.DataProviderSelection == _selectedItem.Step,
			Abbriviation = "1",
		});
		_steps.Add(new TfMenuItem()
		{
			Id= $"tf-{Guid.NewGuid()}",
			Text = LOC("Provider Options"),
			Description = LOC("method configuration"),
			Selected = TfImportFileToPageContextItemStep.DataProviderOptions == _selectedItem.Step,
			Abbriviation = "2",
		});		
		_steps.Add(new TfMenuItem()
		{
			Id= $"tf-{Guid.NewGuid()}",
			Text = LOC("Create Page"),
			Description = LOC("required items creation"),
			Selected = TfImportFileToPageContextItemStep.PageCreation == _selectedItem.Step,
			Abbriviation = "3",
		});				
		_steps.Add(new TfMenuItem()
		{
			Id= $"tf-{Guid.NewGuid()}",
			Text = LOC("Completed"),
			Description = LOC("process complete"),
			Selected = TfImportFileToPageContextItemStep.Finished == _selectedItem.Step,
			Abbriviation = "4",
		});				
	}

	private async Task _menuItemClickHandler(TfImportFileToPageContextItem item)
	{
		_selectedItem = item;
		if (item.Status == TfImportFileToPageContextItemStatus.ProcessedWithError)
			item.Status = TfImportFileToPageContextItemStatus.NotStarted;
		else
		{
			item.Status = (TfImportFileToPageContextItemStatus)((int)item.Status + 1);
		}

		_initMenu();
		await InvokeAsync(StateHasChanged);
	}

	private async Task _removeCurrent()
	{
		_context.Items.Remove(_selectedItem);
		if (_context.Items.Count == 0)
		{
			await _cancel();
			return;
		}

		_selectedItem = _context.Items[0];
		_initMenu();
		await InvokeAsync(StateHasChanged);
	}
}