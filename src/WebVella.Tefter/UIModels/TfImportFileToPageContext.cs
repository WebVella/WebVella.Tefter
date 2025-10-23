namespace WebVella.Tefter.Models;

public record TfImportFileToPageContext
{
	public List<ITfDataProviderAddon> DataProviderOptions { get; set; } = new();
	public List<TfImportFileToPageContextItem> Items { get; set; } = new();

	public TfImportFileToPageContext(List<FluentInputFileEventArgs> files)
	{
		foreach (var file in files)
		{
			Items.Add(new TfImportFileToPageContextItem()
			{
				File = file
			});
		}
	}
}


public record TfImportFileToPageContextItem
{
	public FluentInputFileEventArgs File { get; set; } = null!;
	public string FileName { get; set; } = String.Empty;
	public string LocalPath { get; set; } = String.Empty;
	public byte[] FileContent { get; set; } = [];
	public ITfDataProviderAddon? DataProvider { get; set; } = null;
}