using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Assets.Models;

public record AssetsFolder
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
    public string DataIdentity { get; set; } = null;
    public string CountSharedColumnName { get; set; } = null;

	public AssetsFolder() :
		this(Guid.Empty, string.Empty)
	{
	}

	public AssetsFolder(
		Guid id,
		string name,
		string dataIdentity = null,
		string countSharedColumnName = null)
	{
		Id = id;
		Name = name;
        DataIdentity = dataIdentity;
		CountSharedColumnName = countSharedColumnName;
	}
}

public record UpdateAssetsFolderModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }

    public UpdateAssetsFolderModel() :
        this(Guid.Empty, string.Empty)
    {
    }

    public UpdateAssetsFolderModel(
        Guid id,
        string name )
    {
        Id = id;
        Name = name;
    }
}