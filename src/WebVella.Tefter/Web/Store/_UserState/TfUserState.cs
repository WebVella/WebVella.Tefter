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
	public TfColor ThemeColor
	{
		get
		{
			if (CurrentUser is not null && CurrentUser.Settings is not null)
				return CurrentUser.Settings.ThemeColor;

			return TfColor.Emerald500;
		}
	}

	public string ThemeColorString
	{
		get
		{
			return ThemeColor.ToColorString();
		}
	}

	public SystemColor ThemeColorObject
	{
		get
		{
			if(ThemeColorString == "default") 
				return (SystemColor)System.Drawing.ColorTranslator.FromHtml(TfConstants.DefaultThemeColor.ToColorString());
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(ThemeColorString);
		}
	}

	public string ThemeBackgkroundColor => ThemeColor != TfColor.Black ? $"{ThemeColorString}25" : "transparent";

	public string ThemeBorderColor => ThemeColor != TfColor.Black ? $"{ThemeColorString}75" : "transparent";

	public string ThemeBackgroundAccentColor => ThemeColor != TfColor.Black ? $"{ThemeColorString}35" : "transparent";

	public string ThemeSidebarStyle => ThemeColor != TfColor.Black ? $"background-color:{ThemeBackgkroundColor} !important; border-color:{ThemeBorderColor} !important" :
		$"background-color:transparent !important; border-color:transparent !important";
}