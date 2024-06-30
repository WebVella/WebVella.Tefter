using WebVella.Tefter.UseCases.State;

namespace WebVella.Tefter.Web.Store.ThemeState;

public record InitThemeStateAction
{
	internal StateUseCase UseCase { get; }
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; } = OfficeColor.Excel;

	internal InitThemeStateAction(
		StateUseCase useCase,
		DesignThemeModes themeMode, 
		OfficeColor themeColor)
	{
		UseCase = useCase;
		ThemeMode = themeMode;
		ThemeColor = themeColor;
	}
}
