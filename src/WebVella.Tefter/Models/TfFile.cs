namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("files")]
public class TfFile
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("filepath")]
	public string FilePath { get; set; }

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
				return new Uri($"tefter://fs{FilePath}");
			}
			catch
			{
				return null;
			}
		} 
	}

	public string DownloadPath { get { return $"/fs{FilePath}"; } }
}