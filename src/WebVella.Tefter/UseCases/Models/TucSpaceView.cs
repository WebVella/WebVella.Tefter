using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceView
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public Guid SpaceId { get; set; }

	[Required]
	public SpaceViewType Type { get; init; } = SpaceViewType.Report;

	[Required]
	public TucSpaceViewDataSetType DataSetType { get; set; } = TucSpaceViewDataSetType.New;
	public Guid? DataProviderId { get; set; } = null;
	public string SpaceDataName { get; set; } = null;
	public Guid? SpaceDataId { get; set; } = null;

	[Required]
	public TucSpaceViewDataSetColumnGenerationType ColumnGenerationType { get; set; } = TucSpaceViewDataSetColumnGenerationType.AllNonSystem;

	public TucSpaceView() { }

	//public TucSpaceView(TfSpaceView model)
	//{
	//	Id = model.Id;
	//	Name = model.Name;
	//	Position = model.Position;
	//	IsPrivate = model.IsPrivate;
	//	IconString = model.Icon;
	//	Color = Web.Utils.EnumExtensions.ConvertIntToEnum<OfficeColor>(model.Color, OfficeColor.Default);
	//}

	//public TfSpace ToModel()
	//{
	//	return new TfSpace
	//	{
	//		Id = Id,
	//		Name = Name,
	//		Position = Position,
	//		IsPrivate = IsPrivate,
	//		Icon = IconString,
	//		Color = (short)Color
	//	};
	//}
}

public enum TucSpaceViewDataSetType { 
	[Description("new dataset")]
	New = 0,
	[Description("existing dataset")]
	Existing = 1
}

public enum TucSpaceViewDataSetColumnGenerationType { 
	[Description("add all non-system provider columns")]
	AllNonSystem = 0,
	[Description("add all provider columns (incl.system)")]
	AllColumns = 1,
	[Description("do not generate columns, to be added later")]
	NoColumns = 2,
}
