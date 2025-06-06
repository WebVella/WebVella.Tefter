﻿namespace WebVella.Tefter.Assets.Models;

public class Asset
{
	public Guid Id { get; set; }
	public Guid FolderId { get; set; }
	public AssetType Type { get; set; }
	public AssetContentBase Content { get; set; }
	public TfUser CreatedBy { get; set; }
	public DateTime CreatedOn { get; set; }
	public TfUser ModifiedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public Dictionary<Guid, string> RelatedSK { get; set; }
}

[JsonDerivedType(typeof(FileAssetContent), typeDiscriminator: "file")]
[JsonDerivedType(typeof(LinkAssetContent), typeDiscriminator: "link")]
[JsonDerivedType(typeof(DocumentAssetContent), typeDiscriminator: "document")]
public class AssetContentBase
{
	public string Label { get; set; }
}

public class FileAssetContent : AssetContentBase
{
	public Guid BlobId { get; set; }
	public string Filename { get; set; }
	public string DownloadUrl { get; set; }
}

public class LinkAssetContent : AssetContentBase
{
	public string Url { get; set; }
	public string IconUrl { get; set; }
}

public class DocumentAssetContent : AssetContentBase
{
	public DocumentAssetContentType Type { get; set; }
	public string Content { get; set; }
}

public class CreateFileAssetModel
{
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string FileName { get; set; }
	public string LocalPath { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
}

public class CreateLinkAssetModel
{
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string Url { get; set; }
	public string IconUrl { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
}

public class CreateFileAssetWithJoinKeyModel
{
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string FileName { get; set; }
	public string LocalPath { get; set; }
	public Guid CreatedBy { get; set; }
	public List<string> SKValues { get; set; }
}

public class CreateLinkAssetWithJoinKeyModel
{
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string Url { get; set; }
	public string IconUrl { get; set; }
	public Guid CreatedBy { get; set; }
	public List<string> SKValues { get; set; }
}