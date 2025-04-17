namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;

[FeatureState]
public partial record TfUserState
{
	public Guid Hash { get; init; } = Guid.NewGuid();
	public Guid SessionId { get; init; } = Guid.NewGuid();
	public TucUser CurrentUser { get; init; }
	public TucCultureOption Culture { get; init; }
	public bool SidebarExpanded
	{
		get
		{
			if (CurrentUser is not null && CurrentUser.Settings is not null)
				return CurrentUser.Settings.IsSidebarOpen;
			return true;
		}
	}
	public DesignThemeModes ThemeMode
	{
		get
		{
			if (CurrentUser is not null && CurrentUser.Settings is not null)
				return CurrentUser.Settings.ThemeMode;

			return DesignThemeModes.System;
		}
	}
	public OfficeColor ThemeColor
	{
		get
		{
			if (CurrentUser is not null && CurrentUser.Settings is not null)
				return CurrentUser.Settings.ThemeColor;

			return OfficeColor.Windows;
		}
	}

	public string ThemeColorString
	{
		get
		{
			return ThemeColor.ToAttributeValue();
		}
	}

	public SystemColor ThemeColorObject
	{
		get
		{
			if(ThemeColorString == "default") 
				return (SystemColor)System.Drawing.ColorTranslator.FromHtml(TfConstants.DefaultThemeColor.ToAttributeValue());
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(ThemeColorString);
		}
	}

	public string ThemeBackgkroundColor => ThemeColor != OfficeColor.Default ? $"{ThemeColorString}25" : "transparent";

	public string ThemeBorderColor => ThemeColor != OfficeColor.Default ? $"{ThemeColorString}75" : "transparent";

	public string ThemeBackgroundAccentColor => ThemeColor != OfficeColor.Default ? $"{ThemeColorString}35" : "transparent";

	public string ThemeSidebarStyle => ThemeColor != OfficeColor.Default ? $"background-color:{ThemeBackgkroundColor} !important; border-color:{ThemeBorderColor} !important" :
		$"background-color:transparent !important; border-color:transparent !important";
}