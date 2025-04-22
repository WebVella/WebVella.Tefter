using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpace
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	public short Position { get; set; }
	public bool IsPrivate { get; set; }
	[Required]
	public string IconString { get; set; } = "Apps";
	[Required]
	public TfColor Color { get; set; } = TfColor.Emerald500;

	public Icon Icon { get => TfConstants.GetIcon(IconString); }

	public Guid? DefaultNodeId { get; set; } = null;

	public string Url
	{
		get
		{
			if(DefaultNodeId is null) return string.Format(TfConstants.SpacePageUrl, Id);

			return string.Format(TfConstants.SpaceNodePageUrl, Id, DefaultNodeId.Value);
		}
	}

	public TucSpace() { }

	public TucSpace(TfSpace model)
	{
		Id = model.Id;
		Name = model.Name;
		Position = model.Position;
		IsPrivate = model.IsPrivate;
		IconString = model.Icon;
		Color = Web.Utils.EnumExtensions.ConvertIntToEnum<TfColor>(model.Color, TfColor.Emerald500);
	}

	public TfSpace ToModel()
	{
		return new TfSpace
		{
			Id = Id,
			Name = Name,
			Position = Position,
			IsPrivate = IsPrivate,
			Icon = IconString,
			Color = (short)Color
		};
	}
}
