namespace WebVella.Tefter;

public partial class TfConstants
{
	public static Icon? GetIcon(string? name, IconSize size = IconSize.Size20,
		IconVariant variant = IconVariant.Regular, string? defaultIcon = null)
	{
		try
		{
			if (String.IsNullOrWhiteSpace(name) && String.IsNullOrWhiteSpace(defaultIcon))
				return null;
			if (String.IsNullOrWhiteSpace(name)) return GetIcon(name: defaultIcon, size: size, variant: variant);
			return IconsExtensions.GetInstance(new IconInfo { Name = name, Size = size, Variant = variant });
		}
		catch
		{
			return IconsExtensions
				.GetInstance(new IconInfo
				{
					Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Regular
				}).WithColor(Color.Error);
		}
	}

	//Action icons
	public static Icon ErrorIcon = IconsExtensions
		.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Filled })
		.WithColor(Color.Error);

	//Storage keys
	public static string SpaceViewOpenedGroupsLocalStorageKey = "tf-spaceview-opened-groups";
}