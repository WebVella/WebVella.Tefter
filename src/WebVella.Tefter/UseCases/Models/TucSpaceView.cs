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
	public TucSpaceViewType Type { get; init; } = TucSpaceViewType.Report;

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

	[JsonIgnore]
	public Action OnClick { get;set;}

	public TucSpaceView() { }

	public TucSpaceView(TfSpaceView model)
	{
		Id = model.Id;
		Name = model.Name;
		SpaceId = model.SpaceId;
		Type = model.Type.ConvertSafeToEnum<TfSpaceViewType,TucSpaceViewType>();
		SpaceDataId = model.SpaceDataId;
		Position = model.Position;
	}

	public TfSpaceView ToModel()
	{
		return new TfSpaceView
		{
			Id = Id,
			Name = Name,
			SpaceId = SpaceId,
			Type = Type.ConvertSafeToEnum<TucSpaceViewType,TfSpaceViewType>(),
			SpaceDataId = SpaceDataId ?? Guid.Empty,
			Position = Position,
		};
	}

	public TfCreateSpaceViewExtended ToModelExtended()
	{
		return new TfCreateSpaceViewExtended
		{
			Id = Id,
			Name = Name,
			SpaceId = SpaceId,
			Type = Type.ConvertSafeToEnum<TucSpaceViewType,TfSpaceViewType>(),
			SpaceDataId = SpaceDataId ?? Guid.Empty,
			Position = Position,
			AddProviderColumns = AddProviderColumns,
			AddSharedColumns = AddSharedColumns,
			AddSystemColumns = AddSystemColumns,
			DataProviderId = DataProviderId,
			NewSpaceDataName = NewSpaceDataName
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

