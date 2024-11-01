using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Assets.Models;

public record Folder
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string SharedKey { get; set; } = null;
	public string CountSharedColumnName { get; set; } = null;

	public Folder() :
		this(Guid.Empty, string.Empty)
	{
	}

	public Folder(
		Guid id,
		string name,
		string sharedKey = null,
		string countSharedColumnName = null)
	{
		Id = id;
		Name = name;
		SharedKey = sharedKey;
		CountSharedColumnName = countSharedColumnName;
	}
}
