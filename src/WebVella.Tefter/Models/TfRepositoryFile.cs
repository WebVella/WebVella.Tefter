namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("repository_file")]
public class TfRepositoryFile
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("filename")]
	public string Filename { get; set; }

	[TfDboModelProperty("created_by")]
	public Guid? CreatedBy { get; set; }

	[TfDboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; }

	[TfDboModelProperty("last_modified_by")]
	public Guid? LastModifiedBy { get; set; }

	[TfDboModelProperty("last_modified_on")]
	public DateTime LastModifiedOn { get; set; }

	public Uri Uri 
	{ 
		get
		{
			try
			{
				return new Uri($"tefter:///fs/repository/{Filename}");
			}
			catch
			{
				return null;
			}
		} 
	}

	public string DownloadPath { get { return $"/fs/repository/{Filename}"; } }

	public override string ToString()
	{
		return Filename;
	}
}