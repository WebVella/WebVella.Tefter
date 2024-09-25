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

	[DboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";
}


public class TfCreateSpaceViewExtended
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.Report;
	public short Position { get; set; }
	public string Name { get; set; }
	public Guid? SpaceDataId { get; set; } = null;
	public string NewSpaceDataName { get; set; } = null;
	public Guid? DataProviderId { get; set; } = null;
	public bool AddSystemColumns { get; set; } = false;
	public bool AddProviderColumns { get; set; } = true;
	public bool AddSharedColumns { get; set; } = true;
}

public enum TfSpaceViewType
{
	Report = 0,
	Form = 1,
	Chart = 2,
	Dashboard = 3
}
