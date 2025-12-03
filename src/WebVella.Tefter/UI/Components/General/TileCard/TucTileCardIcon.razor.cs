namespace WebVella.Tefter.UI.Components;

public partial class TucTileCardIcon : TfBaseComponent
{
	[Parameter] public string? FluentIconName { get; set; } = null;
	[Parameter] public TfColor? Color { get; set; } = null;

	private string _color
	{
		get
		{
			if (Color is not null)
				return Color.GetColor().OKLCH;
			else
			{
				var userColor = TfAuthLayout.GetState().User.Settings.Color;
				return userColor.GetColor().OKLCH;
			}
		}
	}
}