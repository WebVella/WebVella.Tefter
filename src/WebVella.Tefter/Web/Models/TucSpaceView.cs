using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceView
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public Guid SpaceId { get; set; }

	[Required]
	public TucSpaceViewType Type { get; init; } = TucSpaceViewType.DataGrid;

	[Required]
	public TucSpaceViewDataSetType DataSetType { get; set; } = TucSpaceViewDataSetType.New;
	public Guid? DataProviderId { get; set; } = null;
	public string NewSpaceDataName { get; set; } = null;
	public Guid? SpaceDataId { get; set; } = null;
	public short Position { get; set; }

	[Required]
	public bool AddSystemColumns { get; set; } = false;
	[Required]
	public bool AddProviderColumns { get; set; } = true;
	[Required]
	public bool AddSharedColumns { get; set; } = true;
	[Required]
	public bool AddDatasetColumns { get; set; } = true;
	public TucSpaceViewSettings Settings { get; set; } = new TucSpaceViewSettings();
	public List<TucSpaceViewPreset> Presets { get; set; } = new();

	[JsonIgnore]
	public Action OnClick { get; set; }

	public TucSpaceView() { }

	public TucSpaceView(TfSpaceView model)
	{
		Id = model.Id;
		Name = model.Name;
		SpaceId = model.SpaceId;
		Type = model.Type.ConvertSafeToEnum<TfSpaceViewType, TucSpaceViewType>();
		SpaceDataId = model.SpaceDataId;
		Position = model.Position;
		Settings = new TucSpaceViewSettings();
		if (!String.IsNullOrWhiteSpace(model.SettingsJson) && model.SettingsJson.StartsWith("{")
		 && model.SettingsJson.EndsWith("}"))
		{
			Settings = JsonSerializer.Deserialize<TucSpaceViewSettings>(model.SettingsJson);
		}
		Presets = model.Presets.Select(x=> new TucSpaceViewPreset(x)).ToList();
		
	}

	public TfSpaceView ToModel()
	{
		return new TfSpaceView
		{
			Id = Id,
			Name = Name,
			SpaceId = SpaceId,
			Type = Type.ConvertSafeToEnum<TucSpaceViewType, TfSpaceViewType>(),
			SpaceDataId = SpaceDataId ?? Guid.Empty,
			Position = Position,
			SettingsJson = JsonSerializer.Serialize(Settings),
			Presets = Presets.Select(x=> x.ToModel()).ToList()
		};
	}

	public TfCreateSpaceViewExtended ToModelExtended()
	{
		return new TfCreateSpaceViewExtended
		{
			Id = Id,
			Name = Name,
			SpaceId = SpaceId,
			Type = Type.ConvertSafeToEnum<TucSpaceViewType, TfSpaceViewType>(),
			SpaceDataId = SpaceDataId,
			Position = Position,
			AddProviderColumns = AddProviderColumns,
			AddSharedColumns = AddSharedColumns,
			AddSystemColumns = AddSystemColumns,
			DataProviderId = DataProviderId,
			NewSpaceDataName = NewSpaceDataName,
			AddDataSetColumns = AddDatasetColumns,
			SettingsJson = JsonSerializer.Serialize(Settings),
			Presets = Presets.Select(x=> x.ToModel()).ToList()
		};
	}
}

public enum TucSpaceViewDataSetType
{
	[Description("new dataset")]
	New = 0,
	[Description("existing dataset")]
	Existing = 1
}

public enum TucSpaceViewSetType
{
	[Description("new view")]
	New = 0,
	[Description("existing view")]
	Existing = 1
}
