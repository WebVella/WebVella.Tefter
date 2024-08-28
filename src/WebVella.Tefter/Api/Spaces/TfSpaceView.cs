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
	[DboTypeConverter(typeof(EnumPropertyConverter<SpaceViewType>))]
	public SpaceViewType Type { get; set; } = SpaceViewType.Report;

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
	public bool? GenerateSpaceDataWithAllColumns { get; set; } = null;
}