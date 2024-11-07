namespace WebVella.Tefter.Web.Models;

public record TucRepositoryFile
{
	public Guid Id { get; set; }

	public string FileName { get; set; }
	public string FileExtension { get => Path.GetExtension(FileName);}

	public Guid? CreatedBy { get; set; }

	public DateTime CreatedOn { get; set; }

	public Guid? LastModifiedBy { get; set; }

	public DateTime LastModifiedOn { get; set; }
	public Uri Uri { get; set; }
	public string DownloadPath { get; set; }
	public string UploadTempPath { get; set; }

	public TucRepositoryFile() { }
	public TucRepositoryFile(TfRepositoryFile model)
	{
		Id = model.Id;
		FileName = model.Filename;
		CreatedBy = model.CreatedBy;
		CreatedOn = model.CreatedOn;
		LastModifiedBy = model.LastModifiedBy;
		LastModifiedOn = model.LastModifiedOn;
		Uri = model.Uri;
		DownloadPath = model.DownloadPath;
	}
	public TfRepositoryFile ToModel()
	{
		return new TfRepositoryFile
		{
			Id = Id,
			Filename = FileName,
			CreatedBy = CreatedBy,
			LastModifiedBy = LastModifiedBy,
			LastModifiedOn = LastModifiedOn,
			CreatedOn = CreatedOn,
		};
	}

	
	
}
