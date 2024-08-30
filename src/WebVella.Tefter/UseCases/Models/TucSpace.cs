using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

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
	public OfficeColor Color { get; set; } = OfficeColor.Default;

	public Icon Icon { get => TfConstants.GetIcon(IconString); }

	public Guid? DefaultViewId { get; set; } = null;

	public TucSpace() { }

	public TucSpace(TfSpace model)
	{
		Id = model.Id;
		Name = model.Name;
		Position = model.Position;
		IsPrivate = model.IsPrivate;
		IconString = model.Icon;
		Color = Web.Utils.EnumExtensions.ConvertIntToEnum<OfficeColor>(model.Color, OfficeColor.Default);
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
