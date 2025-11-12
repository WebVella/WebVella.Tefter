namespace WebVella.Tefter.Models;

public record TfSpacePageCreateFromFileContext
{
	public List<ITfDataProviderAddon> DataProviderOptions { get; set; } = new();
	public List<TfSpacePageCreateFromFileContextItem> Items { get; set; } = new();

	public TfSpacePageCreateFromFileContext(List<FluentInputFileEventArgs> files)
	{
		foreach (var file in files)
		{
			if (file.LocalFile is null)
				throw new Exception("one or more files have no content");

			using var fileStream =
				new FileStream(file.LocalFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);

			var memoryStream = new MemoryStream();
			fileStream.Position = 0;
			fileStream.CopyTo(memoryStream);
			memoryStream.Position = 0;

			if (Items.Any(x => x.LocalPath == file.LocalFile.FullName)) continue;

			Items.Add(new TfSpacePageCreateFromFileContextItem()
			{
				File = file, FileName = file.Name, LocalPath = file.LocalFile!.FullName, FileContent = memoryStream,
			});
			try
			{
				File.Delete(file.LocalFile.FullName);
			}
			catch (Exception)
			{
				// ignored
			}
		}
	}
}

public record TfSpacePageCreateFromFileContextItem
{
	public TfUser User { get; set; } = null!;
	public TfSpace Space { get; set; } = null!;
	public FluentInputFileEventArgs File { get; set; } = null!;
	public string FileName { get; set; } = String.Empty;
	public string LocalPath { get; set; } = String.Empty;
	public MemoryStream? FileContent { get; set; } = null;
	public bool IsSuccess { get; set; } = false;
	public bool IsProcessed { get; set; } = false;

	public TfImportFileToPageContextItemStatus Status
	{
		get
		{
			if (!IsProcessed)
				return TfImportFileToPageContextItemStatus.Processing;

			if (ProcessStream.GetProgressLog().Count == 0)
				return TfImportFileToPageContextItemStatus.NotStarted;

			if (ProcessStream.GetProgressLog().Any(x => x.Type == TfProgressStreamItemType.Error))
				return TfImportFileToPageContextItemStatus.ProcessedWithErrors;

			if (ProcessStream.GetProgressLog().Any(x => x.Type == TfProgressStreamItemType.Warning))
				return TfImportFileToPageContextItemStatus.ProcessedWithWarnings;

			return TfImportFileToPageContextItemStatus.ProcessedSuccess;
		}
	}

	public TfProgressStream ProcessStream { get; set; } = new();

	public Icon GetStatusIcon()
	{
		switch (Status)
		{
			case TfImportFileToPageContextItemStatus.Processing:
				return TfConstants.GetIcon("SpinnerIos", variant: IconVariant.Filled)!.WithColor(TfColor.Green500
					.GetColor().HEX);
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

	public TfImportFileToPageResultProcessContext ProcessContext { get; set; } = new();
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

public record TfImportFileToPageResultProcessContext
{
	public ITfDataProviderAddon? UsedDataProviderAddon { get; set; } = null;
	public TfDataProviderSourceSchemaInfo? DataSchemaInfo { get; set; } = null;
	public TfImportFileToPageDataProviderCreationRequest? DataProviderCreationRequest { get; set; } = null;
	public TfRepositoryFile? RepositoryFile { get; set; } = null;
	public TfDataProvider? DataProvider { get; set; } = null;
	public List<string> CreatedRepositoryFiles { get; set; } = new();
}

public record TfImportFileToPageDataProviderCreationRequest
{
	public string? Name { get; set; } = null;
	public string? SettingsJson { get; set; } = null;
	public List<string> SynchPrimaryKeyColumns { get; set; } = new();
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;
}