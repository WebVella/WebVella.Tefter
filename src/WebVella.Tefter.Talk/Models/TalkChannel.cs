using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Talk.Models;

public record TalkChannel
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string DataIdentity { get; set; } = null;
	public string CountSharedColumnName { get; set; } = null;

	public TalkChannel() :
		this(Guid.Empty, string.Empty)
	{
	}

	public TalkChannel(
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
