namespace WebVella.Tefter.Web.Models;

public record TucUserSettings
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;

	public OfficeColor ThemeColor { get; init; } = OfficeColor.Windows;

	public bool IsSidebarOpen { get; init; } = true;

	public string CultureName { get; init; } = string.Empty;
	public string StartUpUrl { get; init; } = string.Empty;
	public int? PageSize { get; init; } = null;

	public TucUserSettings(){}
	public TucUserSettings(TfUserSettings model){
		ThemeMode = model.ThemeMode;
		ThemeColor = model.ThemeColor;
		IsSidebarOpen = model.IsSidebarOpen;
		CultureName = model.CultureName;
		StartUpUrl = model.StartUpUrl;
		PageSize = model.PageSize;
	}

	public TfUserSettings ToModel()
	{
		return new TfUserSettings
		{
			CultureName = CultureName,
			ThemeMode = ThemeMode,
			IsSidebarOpen = IsSidebarOpen,
			ThemeColor = ThemeColor,
			StartUpUrl = StartUpUrl,
			PageSize = PageSize,
		};
	}
}
