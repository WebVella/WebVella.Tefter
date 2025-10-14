namespace WebVella.Tefter.UI.Components;
public partial class TucPlainTextField : TfBaseComponent
{
	[Parameter] public string Text { get; set; }
	[Parameter] public string IconName { get; set; }
	[Parameter] public string IconColor { get; set; }


	private Icon? _icon
	{
		get
		{
			if (String.IsNullOrWhiteSpace(IconName)) return null;
			var icon = TfConstants.GetIcon(IconName)!;
			if(String.IsNullOrWhiteSpace(IconColor)) return icon;
			return icon.WithColor(IconColor);
		
		}
	}
}
