using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceView
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }

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
