namespace WebVella.Tefter;

[DboCacheModel]
[DboModel("space_view")]
public class TfSpaceView
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("space_data_id")]
	public Guid SpaceDataId { get; set; }

	[DboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[DboModelProperty("type")]
	[DboTypeConverter(typeof(EnumPropertyConverter<TfSpaceViewType>))]
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.Report;

	[DboModelProperty("name")]
	public string Name { get; set; }


	[DboModelProperty("position")]
	public short Position { get; set; }
}


public class TfCreateSpaceViewExtended
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public SpaceViewType Type { get; set; } = SpaceViewType.Report;
	public string Name { get; set; }
	public Guid? SpaceDataId { get; set; } = null;
	public string NewSpaceDataName { get; set; } = null;
	public Guid? DataProviderId { get; set; } = null;
	public TfSpaceViewColumnGenerationType ColumnsGenerationType { get; set; } =  TfSpaceViewColumnGenerationType.AllNonSystem;
}

public enum TfSpaceViewColumnGenerationType
{
	AllNonSystem = 0,
	AllColumns = 1,
	NoColumns = 2,
}

public enum TfSpaceViewType
{
	Report = 0,
	Form = 1,
	Chart = 2,
	Dashboard = 3
}
