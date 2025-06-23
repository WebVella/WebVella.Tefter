using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceDataIdentity
{
	public Guid Id { get; internal set; }

	public Guid SpaceDataId { get; internal set; }

	public string Name { get; set; }

	public bool IsSystem { get { return Name == TfConstants.TF_ROW_ID_DATA_IDENTITY;  } }

	public List<string> Columns { get; internal set; } = new();

	public TucSpaceDataIdentity() { }

	public TucSpaceDataIdentity(TfSpaceDataIdentity model)
	{
		Id = model.Id;
		SpaceDataId = model.SpaceDataId;
		Name = model.DataIdentity;
		Columns = model.Columns;
	}

	public TfSpaceDataIdentity ToModel()
	{
		return new TfSpaceDataIdentity
		{
			Id = Id,
			SpaceDataId= SpaceDataId,
			DataIdentity= Name,
			Columns = Columns,
		};
	}
}
