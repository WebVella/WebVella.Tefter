namespace WebVella.Tefter.Assets.Models;

public class Asset
{
	public Guid Id { get; set; }
	public Guid FolderId { get; set; }
	public AssetType Type { get; set; }
	public AssetContentBase Content { get; set; }
	public User CreatedBy { get; set; }
	public DateTime CreatedOn { get; set; }
	public User ModifiedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public Dictionary<Guid, string> RelatedSK { get; set; }
}

[JsonDerivedType(typeof(FileAssetContent), typeDiscriminator: "file")]
[JsonDerivedType(typeof(LinkAssetContent), typeDiscriminator: "link")]
public class AssetContentBase
{
}

public class FileAssetContent : AssetContentBase
{
	public string FilePath { get; set; }
}

public class LinkAssetContent : AssetContentBase
{
	public string Url { get; set; }
}


public class CreateAssetModel
{
	public Guid FolderId { get; set; }
	public AssetType Type { get; set; }
	public AssetContentBase Content { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
}

public class CreateAssetWithSharedKeyModel
{
	public Guid FolderId { get; set; }
	public AssetType Type { get; set; }
	public AssetContentBase Content { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> SKValueIds { get; set; }
}