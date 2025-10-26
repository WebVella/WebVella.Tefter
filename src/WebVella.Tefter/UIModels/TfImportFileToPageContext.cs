namespace WebVella.Tefter.Models;

public record TfImportFileToPageContext
{
	public List<ITfDataProviderAddon> DataProviderOptions { get; set; } = new();
	public List<TfImportFileToPageContextItem> Items { get; set; } = new();

	public TfImportFileToPageContext(List<FluentInputFileEventArgs> files)
	{
		foreach (var file in files)
		{
			if (file.LocalFile is null)
				throw new Exception("one or more files have no content");

			byte[] fileContent;
			using (var fileStream =
			       new FileStream(file.LocalFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				fileContent = fileStream.ReadFully();
			}

			if (Items.Any(x => x.LocalPath == file.LocalFile.FullName)) continue;

			Items.Add(new TfImportFileToPageContextItem()
			{
				File = file, FileName = file.Name, LocalPath = file.LocalFile!.FullName, FileContent = fileContent,
			});
			try
			{
				File.Delete(file.LocalFile.FullName);
			}
			catch (Exception ex)
			{
			}
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
	public Guid? SpacePageId { get; set; } = null;
	public bool IsProcessed { get; set; } = false;	
	public TfImportFileToPageContextItemStatus Status
	{
		get
		{
			if (IsProcessed) 
				return TfImportFileToPageContextItemStatus.Processing;
				
			if(ProcessLog.Count == 0)
				return TfImportFileToPageContextItemStatus.NotStarted;
			
			if(ProcessLog.Any(x=> x.Type == TfProgressStreamItemType.Error))
				return TfImportFileToPageContextItemStatus.ProcessedWithErrors;
			
			if(ProcessLog.Any(x=> x.Type == TfProgressStreamItemType.Warning))
				return TfImportFileToPageContextItemStatus.ProcessedWithWarnings;
			
			return TfImportFileToPageContextItemStatus.ProcessedSuccess;	
		}
	}
	public TfProgressStream ProcessStream { get; set; } = new();
	public List<TfProgressStreamItem> ProcessLog { get; set; } = new();

	public Icon GetStatusIcon()
	{
		switch (Status)
		{
			case TfImportFileToPageContextItemStatus.Processing:
				return TfConstants.GetIcon("SpinnerIos",variant:IconVariant.Filled)!.WithColor(TfColor.Green500.GetColor().HEX);			
			case TfImportFileToPageContextItemStatus.ProcessedSuccess:
				return TfConstants.GetIcon("CheckmarkCircle")!.WithColor(TfColor.Green500.GetColor().HEX);
			case TfImportFileToPageContextItemStatus.ProcessedWithWarnings:
				return TfConstants.GetIcon("Warning")!.WithColor(TfColor.Orange500.GetColor().HEX);
			case TfImportFileToPageContextItemStatus.ProcessedWithErrors:
				return TfConstants.GetIcon("ErrorCircle")!.WithColor(TfColor.Red500.GetColor().HEX);
			default:
				return TfConstants.GetIcon("PauseCircle")!.WithColor(TfColor.Gray500.GetColor().HEX);
		}
	}

	public TfColor GetStatusColor()
	{
		switch (Status)
		{
			case TfImportFileToPageContextItemStatus.Processing:
				return TfColor.Cyan500;			
			case TfImportFileToPageContextItemStatus.ProcessedSuccess:
				return TfColor.Green500;
			case TfImportFileToPageContextItemStatus.ProcessedWithWarnings:
				return TfColor.Orange500;
			case TfImportFileToPageContextItemStatus.ProcessedWithErrors:
				return TfColor.Red500;
			default:
				return TfColor.Gray500;
		}
	}

	public bool IsSelected { get; set; } = false;
	public string? Message { get; set; } = null;
	public bool IsError { get; set; } = false;
}

public enum TfImportFileToPageContextItemStatus
{
	[Description("Not Started")] NotStarted = 0,
	[Description("Processing")] Processing = 1,
	[Description("Processed")] ProcessedSuccess = 2,

	[Description("Processed with Warning")]
	ProcessedWithWarnings = 3,
	[Description("Processed with Error")] ProcessedWithErrors = 4
}

public record TfImportFileToPageResult
{
}