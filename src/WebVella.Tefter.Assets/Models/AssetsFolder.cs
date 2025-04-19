using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Assets.Models;

public record AssetsFolder
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string JoinKey { get; set; } = null;
	public string CountSharedColumnName { get; set; } = null;

	public AssetsFolder() :
		this(Guid.Empty, string.Empty)
	{
	}

	public AssetsFolder(
		Guid id,
		string name,
		string joinKey = null,
		string countSharedColumnName = null)
	{
		Id = id;
		Name = name;
		JoinKey = joinKey;
		CountSharedColumnName = countSharedColumnName;
	}
}
