namespace WebVella.Tefter.UseCases.Models;

public record TucUserSettings
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;

	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;

	public bool IsSidebarOpen { get; init; } = true;

	public string CultureName { get; init; } = string.Empty;
	public string StartUpUrl { get; init; } = string.Empty;
	public int? PageSize { get; init; } = null;

	public TucUserSettings(){}
	public TucUserSettings(UserSettings model){
		ThemeMode = model.ThemeMode;
		ThemeColor = model.ThemeColor;
		IsSidebarOpen = model.IsSidebarOpen;
		CultureName = model.CultureName;
		StartUpUrl = model.StartUpUrl;
		PageSize = model.PageSize;
	}

	public UserSettings ToModel()
	{
		return new UserSettings
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
