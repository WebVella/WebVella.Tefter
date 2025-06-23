using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceData
{
	public Guid Id { get; set; }
	[Required]
	public Guid DataProviderId { get; set; }
	[Required]
	public Guid SpaceId { get; set; }
	[Required]
	public string Name { get; set; }
	public short Position { get; set; }
	public List<TucFilterBase> Filters { get; set; } = new();
	public List<string> Columns { get; set; } = new();
	public List<TucSort> SortOrders { get; set; } = new();
	public List<TucSpaceDataIdentity> Identities { get; internal set; }

	public List<string> TotalColumns
	{
		get
		{
			var result = new List<string>();

			result.AddRange(Columns ?? new List<string>());

			foreach (var identity in Identities ?? new List<TucSpaceDataIdentity>())
			{
				foreach (var column in identity.Columns ?? new List<string>())
				{
					result.Add($"{identity.Name}.{column}");
				}
			}
			return result.Order().ToList();
		}
	}

	public TucSpaceData() { }

	public TucSpaceData(TfSpaceData model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		SpaceId = model.SpaceId;
		Name = model.Name;
		Position = model.Position;
		Filters = model.Filters.Select(x => TucFilterBase.FromModel(x)).ToList();
		Columns = model.Columns;
		SortOrders = model.SortOrders.Select(x => new TucSort(x)).ToList();
		Identities = model.Identities.Select(x => new TucSpaceDataIdentity(x)).ToList();

	}

	public TfSpaceData ToModel()
	{
		return new TfSpaceData
		{
			Id = Id,
			DataProviderId = DataProviderId,
			SpaceId = SpaceId,
			Name = Name,
			Position = Position,
			Filters = Filters.Select(x => TucFilterBase.ToModel(x)).ToList(),
			Columns = Columns,
			SortOrders = SortOrders.Select(x => x.ToModel()).ToList(),
			Identities = Identities.Select(x => x.ToModel()).ToList().AsReadOnly()
		};
	}

	public TfUpdateSpaceData ToUpdateModel()
	{
		return new TfUpdateSpaceData
		{
			Id = Id,
			DataProviderId = DataProviderId,
			Name = Name,
			Filters = Filters.Select(x => TucFilterBase.ToModel(x)).ToList(),
			Columns = Columns,
			SortOrders = SortOrders.Select(x => x.ToModel()).ToList(),

		};
	}
}
