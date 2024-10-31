namespace WebVella.Tefter.Web.Models;

public record TucFile
{
	public Guid Id { get; set; }

	public string FilePath { get; set; }
	public string FileName { get => Path.GetFileName(FilePath);}
	public string FileExtension { get => Path.GetExtension(FilePath);}

	public Guid? CreatedBy { get; set; }

	public DateTime CreatedOn { get; set; }

	public Guid? LastModifiedBy { get; set; }

	public DateTime LastModifiedOn { get; set; }
	public Uri Uri { get; set; }
	public string DownloadPath { get; set; }
	public string UploadTempPath { get; set; }

	public TucFile() { }
	public TucFile(TfFile model)
	{
		Id = model.Id;
		FilePath = model.FilePath;
		CreatedBy = model.CreatedBy;
		CreatedOn = model.CreatedOn;
		LastModifiedBy = model.LastModifiedBy;
		LastModifiedOn = model.LastModifiedOn;
		Uri = model.Uri;
		DownloadPath = model.DownloadPath;
	}
	public TfFile ToModel()
	{
		return new TfFile
		{
			Id = Id,
			FilePath = FilePath,
			CreatedBy = CreatedBy,
			LastModifiedBy = LastModifiedBy,
			LastModifiedOn = LastModifiedOn,
			CreatedOn = CreatedOn,
		};
	}

	
	
}
