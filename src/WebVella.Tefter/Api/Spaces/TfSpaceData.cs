namespace WebVella.Tefter;

public class TfSpaceData
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public short Position { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new List<TfFilterBase>();
}


[DboCacheModel]
[DboModel("space_data")]
internal class TfSpaceDataDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("space_id")]
	public Guid SpaceId { get; set; }
	
	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("position")]
	public short Position { get; set; }

	[DboModelProperty("filters_json")]
	public string FiltersJson { get; set; }
}
