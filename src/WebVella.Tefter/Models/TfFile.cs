namespace WebVella.Tefter.Models;

[DboCacheModel]
[DboModel("files")]
public class TfFile
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("filepath")]
	public string FilePath { get; set; }

	[DboModelProperty("created_by")]
	public Guid? CreatedBy { get; set; }

	[DboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; }

	[DboModelProperty("last_modified_by")]
	public Guid? LastModifiedBy { get; set; }

	[DboModelProperty("last_modified_on")]
	public DateTime LastModifiedOn { get; set; }
}